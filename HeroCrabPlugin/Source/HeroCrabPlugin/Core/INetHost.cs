using System;
using HeroCrabPlugin.Sublayer.Udp;

namespace HeroCrabPlugin.Core
{
    public interface INetHost : IDisposable
    {
        event NetHost.NetLogWriteHandler LogWrite;
        void Start(string ipAddress, ushort port);
        void Stop();
        void Process(float time);
    }
}
