using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace SharedLibrary
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly string _hostName;

        public RedisStorage()
        {
            _hostName = Constants.HostName;
            _connection = ConnectionMultiplexer.Connect(_hostName);
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
            return _connection.GetServer(_hostName, Constants.Port).Keys().Select(x => x.ToString());
        }

        public bool IsKeyExist(string key)
        {
            var db = _connection.GetDatabase();
            return db.KeyExists(key);
        }
    }
}