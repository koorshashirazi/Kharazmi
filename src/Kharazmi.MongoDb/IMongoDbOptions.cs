namespace Kharazmi.MongoDb
{
    public interface IMongoDbOptions<TOptionsType>
    {
        string ApplicationName { get; set; }
        string Host { get; set; }
        int Port { get; set; }
        string ReplicaSetName { get; set; }
        string Database { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string ConnectionString { get; set; }
    }
}