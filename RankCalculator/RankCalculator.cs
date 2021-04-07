using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NATS.Client;
using SharedLibrary;

namespace RankCalculator
{
    public class RankCalculator : IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private readonly IAsyncSubscription _subscription;

        public RankCalculator(ILogger logger, IStorage storage)
        {
            _logger = logger;
            _connection = NatsFactory.GetNatsConnection();
            _subscription = _connection.SubscribeAsync(Constants.RankKeyProcessing, "rank", (_, args) =>
            {
                var id = Encoding.UTF8.GetString(args.Message.Data);
                var shard = storage.LoadShard(id);
                var textKey = Constants.TextKeyPrefix + id;
                if (!storage.IsKeyExist(shard, textKey)) return;
                var text = storage.Load(shard, textKey);
                var rank = GetRank(text);

                _logger.LogInformation($"Shard: [{shard}], id: [{id}], text: \"{text}\", rank: [{rank}]");
                storage.Store(shard, Constants.RankKeyPrefix + id, rank.ToString());

                _connection.Publish(Constants.RankKeyCalculated,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new RankObject {Id = id, Value = rank})));
            });
        }

        public void Dispose()
        {
            _logger.LogInformation("RankCalculator is disposing...");
            _subscription.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Subscribe()
        {
            _logger.LogInformation("RankCalculator subscriptions started");
            _subscription.Start();
        }

        private static double GetRank(string text)
        {
            if (text.Length == 0) return 0d;
            return 1d * text.Count(x => !char.IsLetter(x)) / text.Length;
        }
    }
}