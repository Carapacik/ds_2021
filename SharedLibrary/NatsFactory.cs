﻿using NATS.Client;

namespace SharedLibrary
{
    public static class NatsFactory
    {
        public static IConnection GetNatsConnection()
        {
            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = Constants.HostName;
            return new ConnectionFactory().CreateConnection(options);
        }
    }
}