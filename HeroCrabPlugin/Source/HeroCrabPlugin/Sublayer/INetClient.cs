using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer
{
    /// <summary>
    /// Network client.
    /// </summary>
    public interface INetClient : INetHost
    {
        /// <summary>
        /// Network Stream which contains all streamed elements and sessions.
        /// </summary>
        INetStreamClient Stream { get; }
    }
}
