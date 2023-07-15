using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Lending.Infrastructure
{
    public class Repository
    {
        private readonly IDatabase _db;
        private readonly JsonCommandsAsync _jsonCommand;

        public Repository()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = redis.GetDatabase();
            _jsonCommand = new JsonCommandsAsync(_db);
        }

        public async Task<bool> Upsert<T>(Guid id, T value)
        {
            return await _jsonCommand.SetAsync(new RedisKey(id.ToString()), new RedisValue(typeof(T).Name) , value);
        }       
        
        public async Task<RedisResult> Get<T>(Guid id, T value)
        {
            return await _jsonCommand.GetAsync(new RedisKey(id.ToString()), path: new RedisValue(typeof(T).Name));
        }


    }
}