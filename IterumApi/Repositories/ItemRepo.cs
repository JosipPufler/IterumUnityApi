using IterumApi.Models;
using IterumApi.Services;
using MongoDB.Driver;

namespace IterumApi.Repositories
{
    public class ItemRepo
    {
        private readonly IMongoCollection<Item> _collection;

        public ItemRepo(MongoDbService dbService)
        {
            _collection = dbService.GetCollection<Item>("items");
        }

        public async Task<List<Item>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Item?> GetByIdAsync(string id) =>
            await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<List<Item>> GetAllByUserIdAsync(long userId) =>
            await _collection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Item> CreateAsync(Item item)
        {
            await _collection.InsertOneAsync(item);
            return item;
        }

        public async Task<bool> UpdateAsync(Item updatedItem)
        {
            var existing = await _collection.Find(x => x.Id == updatedItem.Id).FirstOrDefaultAsync();

            if (existing == null)
                return false;

            if (existing.UserId != updatedItem.UserId)
                return false;

            var result = await _collection.ReplaceOneAsync(x => x.Id == updatedItem.Id, updatedItem);
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
