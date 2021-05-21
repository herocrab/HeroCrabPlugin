using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace HeroCrabPlugin.Stream
{
    /// <inheritdoc />
    public interface INetStreamServer : INetStream
    {
        /// <summary>
        /// Kick all connected sessions.
        /// </summary>
        void KickAll();

        /// <summary>
        /// Create a network session given a sublayer.
        /// </summary>
        /// <param name="netSublayer">Sublayer</param>
        /// <returns></returns>
        NetSessionServer CreateSession(INetSublayer netSublayer);

        /// <summary>
        /// Create a network element on the server for the stream.
        /// </summary>
        /// <param name="name">String</param>
        /// <param name="assetId">UInt</param>
        /// <param name="authorId">UInt</param>
        /// <param name="isEnabled">Bool</param>
        /// <returns></returns>
        INetElement CreateElement(string name, uint assetId, uint authorId, bool isEnabled);

        /// <summary>
        /// Delete a network element from the stream on the server.
        /// </summary>
        /// <param name="element">INetElement</param>
        void DeleteElement(INetElement element);

        /// <summary>
        /// Find a session given an id.
        /// </summary>
        /// <param name="id">UInt</param>
        /// <param name="session">INetSession</param>
        /// <returns></returns>
        bool FindSession(uint id, out INetSession session);
    }
}
