namespace Kharazmi.MongoDb
{
    public abstract class MongoDbOptions<TOptionsType> : IMongoDbOptions<TOptionsType> where TOptionsType : class
    {
        public string ApplicationName { get; set; }
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 27017;
        public string ReplicaSetName { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
    }
}