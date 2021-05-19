using System.Collections.Generic;

namespace HeroCrabPlugin.Core
{
    public class NetLoggerBuffer : INetLogger
    {
        public int Length => _log.Count;

        private readonly int _logCapacity;
        private uint _logIndex;

        private readonly List<string> _log = new List<string>();

        public NetLoggerBuffer(int logCapacity)
        {
            _logCapacity = logCapacity;
        }

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
