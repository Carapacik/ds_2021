using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection;

        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect(Constants.Host);
        }

        public void Store(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }

        public string Load(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }

        public List<string> GetKeys()
        {
            var keys = _connection.GetServer(Constants.Host, Constants.Port).Keys();

            return keys.Select(x => x.ToString()).ToList();
        }
    }
}