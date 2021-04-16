using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace SharedLibrary
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connectionMain;
        private readonly Dictionary<string, IConnectionMultiplexer> _connections;

        public RedisStorage()
        {
            _connectionMain = ConnectionMultiplexer.Connect(Constants.HostName);
            _connections = new Dictionary<string, IConnectionMultiplexer>
            {
                {
                    Constants.ShardIdRus,
                    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.ShardIdRus))
                },
                {
                    Constants.ShardIdEu,
                    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.ShardIdEu))
                },
                {
                    Constants.ShardIdOther,
                    ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.ShardIdOther))
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
            _connectionMain.GetDatabase().StringSet(key, shard);
        }

        public string Load(string shard, string key)
        {
            var db = _connections[shard].GetDatabase();
            return db.StringGet(key);
        }

        public string LoadShard(string key)
        {
            return _connectionMain.GetDatabase().StringGet(key);
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