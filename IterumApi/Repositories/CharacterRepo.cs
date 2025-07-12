using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Services;
using MongoDB.Driver;

namespace IterumApi.Repositories
{
    public class CharacterRepo
    {
        private readonly IMongoCollection<Character> _collection;

        public CharacterRepo(MongoDbService dbService)
        {
            _collection = dbService.GetCollection<Character>("characters");
        }

        public async Task<List<Character>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Character?> GetByIdAsync(string id) =>
            await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<List<Character>> GetAllByUserIdAsync(long userId) =>
            await _collection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Character> CreateAsync(Character map)
        {
            await _collection.InsertOneAsync(map);
            return map;
        }

        public async Task<bool> UpdateAsync(Character updatedMap)
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
