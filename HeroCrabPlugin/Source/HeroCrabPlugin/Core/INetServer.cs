using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network server.
    /// </summary>
    public interface INetServer : INetHost
    {
        /// <summary>
        /// Network Stream which contains all streamed elements and sessions.
        /// </summary>
        INetStreamServer Stream { get; }

        /// <summary>
        /// Kick all connected sessions from the server.
        /// </summary>
        void KickAll();
    }
}
