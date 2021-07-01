// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Host base class for client and server.
    /// </summary>
    public class NetHost
    {
        /// <summary>
        /// Event that is invoked when a log message is written.
        /// </summary>
        public event NetLogWriteHandler LogWrite;

        /// <summary>
        /// Host network configuration.
        /// </summary>
        protected readonly NetSettings NetSettings;

        /// <summary>
        /// Host network logger.
        /// </summary>
        protected readonly NetLogger NetLogger;

        private const int LogCapacity = 1000;

        /// <summary>
        /// Network host contains the stream and other necessary subsystems.
        /// </summary>
        /// <param name="netSettings"></param>
        protected NetHost(NetSettings netSettings)
        {
            var netLogger = new NetLogger(new NetLoggerBuffer(LogCapacity));

            NetSettings = netSettings;
            NetLogger = netLogger;
            NetLogger.LogWrite += OnLogWrite;

            NetServices.Registry.Add(netSettings);
            NetServices.Registry.Add(netLogger);
        }

        private void OnLogWrite(object sender, string message)
        {
            LogWrite?.Invoke($"{sender}:{message}");
        }
    }
}
