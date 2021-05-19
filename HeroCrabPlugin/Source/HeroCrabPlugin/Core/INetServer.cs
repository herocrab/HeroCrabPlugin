using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Core
{
    public interface INetServer : INetHost
    {
        INetStreamServer Stream { get; }
        void KickAll();
    }
}
