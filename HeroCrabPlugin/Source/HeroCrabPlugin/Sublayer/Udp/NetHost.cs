using HeroCrabPlugin.Core;

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

        /// <summary>
        /// Event that is invoked when a log message is written.
        /// </summary>
        public event NetLogWriteHandler LogWrite;

        /// <summary>
        /// Host network configuration.
        /// </summary>
        protected readonly NetConfig NetConfig;

        /// <summary>
        /// Host network logger.
        /// </summary>
        protected readonly NetLogger NetLogger;

        private const int LogCapacity = 1000;

        /// <summary>
        /// Network host contains the stream and other necessary subsystems.
        /// </summary>
        /// <param name="netConfig"></param>
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
