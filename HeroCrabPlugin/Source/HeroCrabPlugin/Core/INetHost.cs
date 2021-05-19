using System;
// ReSharper disable once CheckNamespace

public interface INetHost : IDisposable
{
    event NetHost.NetLogWriteHandler LogWrite;
    void Start(string ipAddress, ushort port);
    void Stop();
    void Process(float time);
}
