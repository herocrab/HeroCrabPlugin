using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Core
{
    public interface INetClient : INetHost
    {
        INetStreamClient Stream { get; }
    }
}
