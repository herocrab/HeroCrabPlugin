using System.Collections.Generic;

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Logging buffer for net logger.
    /// </summary>
    public class NetLoggerBuffer : INetLogger
    {
        /// <summary>
        /// Count of entries in the log.
        /// </summary>
        public int Length => _log.Count;

        private readonly int _logCapacity;
        private uint _logIndex;

        private readonly List<string> _log = new List<string>();

        /// <summary>
        /// Logging buffer for net logger.
        /// </summary>
        /// <param name="logCapacity"></param>
        public NetLoggerBuffer(int logCapacity)
        {
            _logCapacity = logCapacity;
        }

        /// <summary>
        /// Write a message to the log.
        /// </summary>
        /// <param name="sender">Sending object or class</param>
        /// <param name="message">Log message text</param>
        public void Write(object sender, string message)
        {
            if (_log.Count >= _logCapacity) {
                _log.RemoveAt(0);
            }

            _log.Add($"{_logIndex:00000}: [{sender.GetType().Name}] {message}");
            _logIndex++;
        }
    }
}
