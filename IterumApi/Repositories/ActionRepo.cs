using IterumApi.Services;
using MongoDB.Driver;
using Action = IterumApi.Models.Action;

namespace IterumApi.Repositories
{
    public class ActionRepo
    {
        private readonly IMongoCollection<Action> _collection;

        public ActionRepo(MongoDbService dbService)
        {
            _collection = dbService.GetCollection<Action>("actions");
        }

        public async Task<List<Action>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Action?> GetByIdAsync(string id) =>
            await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<List<Action>> GetAllByUserIdAsync(long userId) =>
            await _collection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Action> CreateAsync(Action action)
        {
            await _collection.InsertOneAsync(action);
            return action;
        }

        public async Task<bool> UpdateAsync(Action updatedAction)
        {
            var existing = await _collection.Find(x => x.Id == updatedAction.Id).FirstOrDefaultAsync();

            if (existing == null)
                return false;

            if (existing.UserId != updatedAction.UserId)
                return false;

            var result = await _collection.ReplaceOneAsync(x => x.Id == updatedAction.Id, updatedAction);
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
