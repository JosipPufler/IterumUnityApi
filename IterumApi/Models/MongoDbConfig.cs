namespace IterumApi.Models
{
    public class MongoDbConfig
    {
        public MongoDbConfig()
        {
            
        }

        public MongoDbConfig(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
