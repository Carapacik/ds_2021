using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NATS.Client;
using SharedLibrary;

namespace EventsLogger
{
    public class EventsLogger : IDisposable
    {
        private readonly IConnection _connection;
        private readonly ILogger<EventsLogger> _logger;
        private readonly IAsyncSubscription _subscription;

        public EventsLogger(ILogger<EventsLogger> logger)
        {
            _logger = logger;
            _connection = NatsFactory.GetNatsConnection();

            _subscription = _connection.SubscribeAsync("rank_calculator.rank_calculated", (sender, args) =>
            {
                var rank = JsonSerializer.Deserialize<RankObject>(args.Message.Data);
                logger.LogInformation($"Event: {args.Message.Subject}\n{rank}");
            });

            _subscription = _connection.SubscribeAsync("valuator.similarity_calculated", (sender, args) =>
            {
                var similarity = JsonSerializer.Deserialize<SimilarityObject>(args.Message.Data);
                logger.LogInformation($"Event: {args.Message.Subject}\n{similarity}");
            });
        }

        public void Dispose()
        {
            _logger.LogInformation("EventLogger is disposing...");
            _subscription.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Subscribe()
        {
            _logger.LogInformation("EventLogger subscriptions started");
            _subscription.Start();
        }
    }
}