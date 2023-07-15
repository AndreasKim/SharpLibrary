using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;

namespace Lending.Infrastructure
{
    public class Repository : IRepository
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
            return await _jsonCommand.SetAsync(new RedisKey(id.ToString()), new RedisValue("$"), value);
        }

        public async Task<T?> Get<T>(Guid id)
        {
            var result = await _jsonCommand.GetAsync(new RedisKey(id.ToString()));
            if (result == null)
                return default;

            return JsonSerializer.Deserialize<T>(result.ToString()!);
        }


    }
}