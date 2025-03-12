using Hangfire.Mongo;

 namespace Kharazmi.BackgroundJob
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoDbStorageOptions : MongoStorageOptions
    {
        /// <summary>
        /// MongoDb connection string 
        /// </summary>
        public string MongoDbConnection { get; set; }
    }
}