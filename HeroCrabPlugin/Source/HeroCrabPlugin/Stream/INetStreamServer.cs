using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;

namespace HeroCrabPlugin.Stream
{
    public interface INetStreamServer : INetStream
    {
        void KickAll();
        NetSessionServer CreateSession(INetSublayer netSublayer);
        INetElement CreateElement(string name, uint assetId, uint authorId, bool isEnabled);
        void DeleteElement(INetElement element);
        bool FindSession(uint id, out INetSession session);
    }
}
