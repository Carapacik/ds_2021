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

        public IEnumerable<string> GetKeys()
        {
            return _connection.GetServer(Constants.Host, Constants.Port).Keys().Select(x => x.ToString()).ToList();
        }

        public bool IsKeyExist(string key)
        {
            var db = _connection.GetDatabase();
            return db.KeyExists(key);
        }
    }
}