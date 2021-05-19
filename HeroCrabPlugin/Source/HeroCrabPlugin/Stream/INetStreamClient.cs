using HeroCrabPlugin.Core;
using HeroCrabPlugin.Session;

namespace HeroCrabPlugin.Stream
{
    public interface INetStreamClient : INetStream
    {
        NetSessionClient CreateSession(INetSublayer netSublayer);
        bool FindSession(out INetSession session);
    }
}
