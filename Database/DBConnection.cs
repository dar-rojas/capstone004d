using MongoDB.Driver;

namespace Api.Database
{
    public class DBConnection
    {
        private static readonly Lazy<DBConnection> _instance =
            new Lazy<DBConnection>(() => new DBConnection());
        
        public static DBConnection Instance => _instance.Value;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        private DBConnection()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not initialize database connection.", ex);
            }
            // Gets the configuration file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var mongoConnectionString = configuration.GetConnectionString("DefaultConnection");
            
            _client = new MongoClient(mongoConnectionString);
            _database = _client.GetDatabase(configuration.GetConnectionString("Database"));
        }

        public IMongoDatabase GetDatabase() => _database;
    }
}