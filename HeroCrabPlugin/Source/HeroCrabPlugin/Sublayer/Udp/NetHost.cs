﻿using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Host base class for client and server.
    /// </summary>
    public class NetHost
    {
        /// <summary>
        /// Handler for the logger.
        /// </summary>
        /// <param name="message">Log message text.</param>
        public delegate void NetLogWriteHandler(string message);

        public event NetLogWriteHandler LogWrite;

        protected readonly NetConfig NetConfig;
        protected readonly NetLogger NetLogger;

        private const int LogCapacity = 1000;

        protected NetHost(NetConfig netConfig)
        {
            var netLogger = new NetLogger(new NetLoggerBuffer(LogCapacity));

            NetConfig = netConfig;
            NetLogger = netLogger;
            NetLogger.LogWrite += OnLogWrite;

            NetServices.Registry.Add(netConfig);
            NetServices.Registry.Add(netLogger);
        }

        private void OnLogWrite(object sender, string message)
        {
            LogWrite?.Invoke($"{sender}:{message}");
        }
    }
}
