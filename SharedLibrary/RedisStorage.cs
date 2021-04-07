using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace SharedLibrary
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly Dictionary<string, IConnectionMultiplexer> _connections;

        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect(Constants.HostName);
            _connections = new Dictionary<string, IConnectionMultiplexer>
            {
                {
                    Constants.ShardIdRus,
                    ConnectionMultiplexer.Connect(Constants.HostName + ":6000")
                },
                {
                    Constants.ShardIdEu,
                    ConnectionMultiplexer.Connect(Constants.HostName + ":6001")
                },
                {
                    Constants.ShardIdOther,
                    ConnectionMultiplexer.Connect(Constants.HostName + ":6002")
                }
            };
        }

        public void Store(string shard, string key, string value)
        {
            var db = _connections[shard].GetDatabase();
            if (key.StartsWith(Constants.TextKeyPrefix)) db.SetAdd(Constants.TextKeyPrefix, value);

            db.StringSet(key, value);
        }

        public void StoreShard(string key, string shard)
        {
            _connection.GetDatabase().StringSet(key, shard);
        }

        public string Load(string shard, string key)
        {
            var db = _connections[shard].GetDatabase();
            return db.StringGet(key);
        }

        public string LoadShard(string key)
        {
            return _connection.GetDatabase().StringGet(key);
        }

        public bool HasTextDuplicates(string text)
        {
            return _connections.Any(x => x.Value.GetDatabase().SetContains(Constants.TextKeyPrefix, text));
        }

        public bool IsKeyExist(string shard, string key)
        {
            var db = _connections[shard].GetDatabase();
            return db.KeyExists(key);
        }
    }
}