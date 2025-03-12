using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

namespace Kharazmi.BackgroundJob
{
    /// <summary> </summary>
    public class JobStoreOptions : DashboardOptions
    {
        /// <summary> </summary>
        public JobStoreOptions()
        {
            ServerOptions = new ServerOptions();
            AuthorizationFilters = new List<IDashboardAuthorizationFilter>
            {
                new LocalRequestsOnlyAuthorizationFilter(),
                new JobStoreDashboardAuthorizationFilter()
            };
            MongoDbStorageOptions = new MongoDbStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                }
            };
        }

        /// <summary> </summary>
        public bool Enable { get; set; }

        /// <summary> </summary>
        public bool EnableDashboard { get; set; }

        /// <summary> </summary>
        public bool UseAuthorizationFilter { get; set; }

        /// <summary> </summary>
        public string PolicyName { get; set; }

        /// <summary> </summary>
        public string PathMatch { get; set; }

        /// <summary> </summary>
        public int CancellationFromSeconds { get; set; }

        /// <summary> </summary>
        public int Attempts { get; set; } = 3;

        /// <summary> </summary>
        public int[] DelaysInSeconds { get; set; } = {5, 10, 15};

        public int JobExpirationFromSeceonds { get; set; } = TimeSpan.FromSeconds(30).Seconds;

        /// <summary> </summary>
        public JobStorageType JobStorageType { get; set; }

        /// <summary> </summary>
        public string RedisConnection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MongoDbStorageOptions MongoDbStorageOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlServerConnection { get; set; }


        /// <summary> </summary>
        public ServerOptions ServerOptions { get; set; }

        /// <summary> </summary>
        public List<IDashboardAuthorizationFilter> AuthorizationFilters { get; set; }

        public string SchemaName { get; set; } = "dbo";
        public int QueuePollIntervalFromSeconds { get; set; } = TimeSpan.FromSeconds(30).Seconds;
        public bool UseRecommendedIsolationLevel { get; set; }
        public bool UsePageLocksOnDequeue { get; set; }
        public bool UseFineGrainedLocks { get; set; }
    }
}