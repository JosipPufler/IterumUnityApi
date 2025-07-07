using IterumApi.Models;
using IterumApi.Services;
using MongoDB.Driver;

namespace IterumApi.Repositories
{
    public class MapRepo
    {
        private readonly IMongoCollection<Map> _collection;

        public MapRepo(MongoDbService dbService)
        {
            _collection = dbService.GetCollection<Map>("maps");
        }

        public async Task<List<Map>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Map?> GetByIdAsync(string id) =>
            await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<List<Map>> GetAllByUserIdAsync(long userId) =>
            await _collection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Map> CreateAsync(Map map)
        {
            await _collection.InsertOneAsync(map);
            return map;
        }

        public async Task<bool> UpdateAsync(Map updatedMap)
        {
            var existing = await _collection.Find(x => x.Id == updatedMap.Id).FirstOrDefaultAsync();

            if (existing == null)
                return false;

            if (existing.UserId != updatedMap.UserId)
                return false;

            var result = await _collection.ReplaceOneAsync(x => x.Id == updatedMap.Id, updatedMap);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id, long userId)
        {
            var existing = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (existing == null)
                return false;

            if (existing.UserId != userId)
                return false;

            var result = await _collection.DeleteOneAsync(u => u.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
