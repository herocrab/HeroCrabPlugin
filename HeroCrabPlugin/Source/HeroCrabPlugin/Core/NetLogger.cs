using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable once CheckNamespace

public class NetLogger
{
    public LoggingGroup Mask { get; set; }

    public delegate void NetLoggerWriteHandler(object sender, string message);
    public event NetLoggerWriteHandler LogWrite;

    [Flags]
    public enum LoggingGroup
    {
        Status = 1,
        Error = 2,
        Session = 4,
        Element = 8,
        Field = 16,
        Custom = 32,
    }

    public int Count => _networkLoggers.Count;

    private readonly List<INetLogger> _networkLoggers = new List<INetLogger>();

    public NetLogger(INetLogger netLogger)
    {
        _networkLoggers.Add(netLogger);

        Mask = LoggingGroup.Status | LoggingGroup.Error | LoggingGroup.Session;
    }

    public void Add(INetLogger netLogger)
    {
        if (_networkLoggers.Contains(netLogger)) {
            _networkLoggers.Remove(netLogger);
        }

        _networkLoggers.Add(netLogger);
    }

    public void Remove(INetLogger netLogger)
    {
        if (_networkLoggers.Contains(netLogger)) {
            _networkLoggers.Remove(netLogger);
        }
    }

    public void Write(LoggingGroup group, object sender, string message)
    {
        if ((Mask & group) != group) {
            return;
        }

        foreach (var networkLogger in _networkLoggers) {
            networkLogger.Write(sender, message);
        }

        var senderName = sender?.ToString()?.Split('.').Last();
        if (string.IsNullOrEmpty(senderName)) {
            senderName = "Null";
        }

        LogWrite?.Invoke(senderName, " " + message);
    }
}
