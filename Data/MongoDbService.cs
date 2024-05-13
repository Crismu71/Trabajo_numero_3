using MongoDB.Driver;

namespace game_freak_pokemon_api.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;
        public MongoDbService(IConfiguration configuration) 
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("DbConnection");
            var mongoUri = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUri);
            _database = mongoClient.GetDatabase(mongoUri.DatabaseName);
        }

        public IMongoDatabase? Database => _database;
    }
}
