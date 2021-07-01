// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Logger for network objects.
    /// </summary>
    public class NetLogger
    {
        /// <summary>
        /// Logger mask level; used for filtering.
        /// </summary>
        public LoggingGroup Mask { get; set; }

        /// <summary>
        /// Handler for log write.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public delegate void NetLoggerWriteHandler(object sender, string message);

        /// <summary>
        /// Event for logging, invoked when a message is logged.
        /// </summary>
        public event NetLoggerWriteHandler LogWrite;

        /// <summary>
        /// Logging group mask, for specifying a logging level and filtering.
        /// </summary>
        [Flags]
        public enum LoggingGroup
        {
            #pragma warning disable 1591
            Status = 1,
            Error = 2,
            Session = 4,
            Element = 8,
            Field = 16,
            Stream = 32,
            Custom = 64,
            #pragma warning restore 1591
        }

        /// <summary>
        /// Log count.
        /// </summary>
        public int Count => _networkLoggers.Count;

        private readonly List<INetLogger> _networkLoggers = new List<INetLogger>();

        /// <summary>
        /// Create a new network logger.
        /// </summary>
        /// <param name="netLogger">Provided logger</param>
        public NetLogger(INetLogger netLogger)
        {
            _networkLoggers.Add(netLogger);

            Mask = LoggingGroup.Status | LoggingGroup.Error | LoggingGroup.Session;
        }

        /// <summary>
        /// Add a logger to the network logger.
        /// </summary>
        /// <param name="netLogger">Provided logger</param>
        public void Add(INetLogger netLogger)
        {
            if (_networkLoggers.Contains(netLogger)) {
                _networkLoggers.Remove(netLogger);
            }

            _networkLoggers.Add(netLogger);
        }

        /// <summary>
        /// Remove a logger from the network logger.
        /// </summary>
        /// <param name="netLogger">Logger</param>
        public void Remove(INetLogger netLogger)
        {
            if (_networkLoggers.Contains(netLogger)) {
                _networkLoggers.Remove(netLogger);
            }
        }

        /// <summary>
        /// Write a log message to the logger.
        /// </summary>
        /// <param name="group">Logging group</param>
        /// <param name="sender">Sending object or class</param>
        /// <param name="message">Log message text</param>
        public void Write(LoggingGroup group, object sender, string message)
        {
            if ((Mask & group) != group) {
                return;
            }

            foreach (var networkLogger in _networkLoggers) {
                networkLogger.Write(sender, message);
            }

            var senderName = sender?.ToString().Split('.').Last();
            if (string.IsNullOrEmpty(senderName)) {
                senderName = "Null";
            }

            LogWrite?.Invoke(senderName, " " + message);
        }
    }
}
