using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kharazmi.MongoDb
{
    public interface IMongoDbContext<TOptions>
    {
        ReadOnlyDictionary<string, Func<Task>> GetCommands { get; }
        int GetCommandsCount { get; }
        IMongoDatabase Database();
        IMongoClient Client();

        Task<IClientSessionHandle> StartSession(ClientSessionOptions options = null,
            CancellationToken cancellationstoken = default);

        IClientSessionHandle Session { get; set; }
        IMongoCollection<TEntity> GetCollection<TEntity>(string name);
        bool Exists(string collectionName);
        Task CreateCollectionAsync(string name, CreateCollectionOptions options);
        Task DropCollectionAsync(string name);
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellactionToken = default);
        bool SaveCommitChanges();
        Task<bool> SaveCommitChangesAsync(CancellationToken cancellationsToken = default);

        Task AddCommandAsync(string key, Func<Task> command);
        void RemoveCommand(string key);
        void RemoveCommands(string[] keys);
        void RemoveCommands();
        TResult RunCommand<TResult>(string command);
        TResult RunCommand<TResult>(string command, ReadPreference readPreference);
        BsonValue RunScript(string command, CancellationToken cancellationToken);
        Task<BsonValue> RunScriptAsync(string command, CancellationToken cancellationToken);
    }
}