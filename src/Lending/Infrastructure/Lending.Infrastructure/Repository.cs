﻿using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using NRedisStack;
using StackExchange.Redis;

namespace Lending.Infrastructure
{
    public class Repository : IRepository
    {
        private readonly IDatabase _db;
        private readonly JsonCommandsAsync _jsonCommand;

        public Repository(string connection)
        {
            var redis = ConnectionMultiplexer.Connect(connection);
            _db = redis.GetDatabase();
            _jsonCommand = new JsonCommandsAsync(_db);
        }

        public async Task<Option<bool>> Upsert<T>(Guid id, T value)
        {
            return await _jsonCommand.SetAsync(new RedisKey(id.ToString()), new RedisValue("$"), value);
        }

        public async Task<Option<T>> Get<T>(Guid id)
        {
            var exists = await _db.KeyExistsAsync(new RedisKey(id.ToString()));

            if(exists)
            {
                var result = await _jsonCommand.GetAsync(new RedisKey(id.ToString()));
                var resultStr = JsonSerializer.Deserialize<T>(result.ToString());
                return Some(resultStr);
            }
            else 
                return None;
        }
    }
}