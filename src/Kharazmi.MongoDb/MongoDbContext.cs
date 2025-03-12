using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Operations;

namespace Kharazmi.MongoDb
{
    public class MongoDbContext<TOptions> : IMongoDbContext<TOptions> where TOptions : class
    {
        private readonly ILogger<MongoDbContext<TOptions>> _logger;

        #region Fields

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly Dictionary<string, Func<Task>> _commands;
        private readonly object _lock = new object();

        private readonly AsyncLock _asyncLock = new AsyncLock();

        #endregion

        #region Ctor

        public MongoDbContext(IMongoDbOptions<TOptions> options, ILogger<MongoDbContext<TOptions>> logger)
        {
            _logger = Ensure.ArgumentIsNotNull(logger, nameof(logger));
            Ensure.ArgumentIsNotNull(options, nameof(options));
            Ensure.IsNotEmpty(options.Database, nameof(options.Database));
            Ensure.IsNotEmpty(options.Host, nameof(options.Host));
            _commands = new Dictionary<string, Func<Task>>();

            var clientSettings = CreateMongoClientSettings(options);
            _client = new MongoClient(clientSettings);
            _database = _client.GetDatabase(options.Database);

            //_client.Cluster.DescriptionChanged += ClusterOnDescriptionChanged;
        }

        #endregion

        #region Methods

        public ReadOnlyDictionary<string, Func<Task>> GetCommands => new ReadOnlyDictionary<string, Func<Task>>(_commands);
        public int GetCommandsCount => _commands.Count;

        public IMongoDatabase Database()
        {
            return _database;
        }

        public IMongoClient Client()
        {
            return _client;
        }

        public Task<IClientSessionHandle> StartSession(ClientSessionOptions options = null,
            CancellationToken cancellationsToken = default)
        {
            return Client().StartSessionAsync(options, cancellationsToken);
        }

        public IClientSessionHandle Session { get; set; }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string name)
        {
            return _database.GetCollection<TEntity>(name);
        }

        public Task AddCommandAsync(string key, Func<Task> command)
        {
            lock (_lock)
            {
                _commands[key] = command;
                return Task.CompletedTask;
            }
        }

        public void RemoveCommand(string key)
        {
            lock (_lock)
                _commands.Remove(key);
        }

        public void RemoveCommands(string[] keys)
        {
            foreach (var key in keys)
                RemoveCommand(key);
        }

        public void RemoveCommands()
        {
            lock (_lock)
                _commands.Clear();
        }

        public Task CreateCollectionAsync(string name, CreateCollectionOptions options)
        {
            _logger.LogInformation($"Create Collection: {name}");

            return Database().CreateCollectionAsync(name, options);
        }

        public Task DropCollectionAsync(string name)
        {
            _logger.LogInformation($"Drop Collection: {name}");
            return Database().DropCollectionAsync(name);
        }

        public int SaveChanges()
        {
            return AsyncHelper.RunSync(() => SaveChangesAsync(CancellationToken.None));
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationsToken = default)
        {
            using (await _asyncLock.WaitAsync(cancellationsToken).ConfigureAwait(false))
            {
                var commandTasks = _commands.Select(c => c.Value()).ToList();
                await Task.WhenAll(commandTasks).ConfigureAwait(false);
                _logger.LogInformation($"CommandsSaveChangesSucceeded: {commandTasks.Count}");
                RemoveCommands();
                return commandTasks.Count;
            }
        }

        public bool SaveCommitChanges()
        {
            return AsyncHelper.RunAsSync(() => SaveCommitChangesAsync(CancellationToken.None));
        }


        public async Task<bool> SaveCommitChangesAsync(CancellationToken cancellationsToken = default)
        {
            using (Session = await StartSession(cancellationsToken: cancellationsToken).ConfigureAwait(false))
            {
                Session.StartTransaction();

                try
                {
                    var commandTasks = _commands.Select(c => c.Value()).ToList();

                    await Task.WhenAll(commandTasks).ConfigureAwait(false);

                    await Session.CommitTransactionAsync(cancellationsToken).ConfigureAwait(false);

                    _logger.LogInformation($"SaveCommitChangesSucceeded: {commandTasks.Count}");
                    RemoveCommands();
                    return commandTasks.Count > 0;
                }
                catch (Exception e)
                {
                    RemoveCommands();
                    await Session.AbortTransactionAsync(cancellationsToken).ConfigureAwait(false);
                    _logger.LogInformation(e.Message);
                    throw new TransactionException(e.Message);
                }
            }
        }


        public TResult RunCommand<TResult>(string command)
        {
            return _database.RunCommand<TResult>(command);
        }

        public TResult RunCommand<TResult>(string command, ReadPreference readPreference)
        {
            return _database.RunCommand<TResult>(command, readPreference);
        }

        public BsonValue RunScript(string command, CancellationToken cancellationToken)
        {
            return AsyncHelper.RunSync(() => RunScriptAsync(command, CancellationToken.None));
        }

        public Task<BsonValue> RunScriptAsync(string command, CancellationToken cancellationToken)
        {
            var script = new BsonJavaScript(command);
            var operation = new EvalOperation(_database.DatabaseNamespace, script, null);
            var writeBinding = new WritableServerBinding(_client.Cluster, NoCoreSession.NewHandle());
            return operation.ExecuteAsync(writeBinding, cancellationToken);
        }

        public bool Exists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions {Filter = filter};
            return Database().ListCollectionNames(options).Any();
        }

        #endregion


        private static MongoClientSettings CreateMongoClientSettings(IMongoDbOptions<TOptions> options)
        {
            MongoClientSettings clientSettings;
            if (options.ConnectionString.IsNotEmpty())
            {
                var mongoUrl = MongoUrl.Create(options.ConnectionString);
                clientSettings = MongoClientSettings.FromUrl(mongoUrl);
            }
            else
            {
                clientSettings = new MongoClientSettings
                {
                    ApplicationName = options.ApplicationName,
                    Servers = new[]
                    {
                        new MongoServerAddress(options.Host, options.Port)
                    }
                };

                if (options.ReplicaSetName.IsNotEmpty())
                {
                    clientSettings.ConnectionMode = ConnectionMode.ReplicaSet;
                    clientSettings.ReplicaSetName = options.ReplicaSetName;
                }

                if (options.UserName.IsNotEmpty() && options.Password.IsNotEmpty())
                {
                    clientSettings.Credential = new MongoCredential("SCRAM-SHA-1",
                        new MongoInternalIdentity(options.Database, options.UserName),
                        new PasswordEvidence(options.Password));
                }
            }

            return clientSettings;
        }

        private void ClusterOnDescriptionChanged(object sender, ClusterDescriptionChangedEventArgs e)
        {
            _logger.LogInformation("New Cluster Id: {0}", e.NewClusterDescription.ClusterId);
        }
    }
}