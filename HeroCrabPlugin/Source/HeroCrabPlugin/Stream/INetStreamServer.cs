// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
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
        /// Create a network session given a sublayer.
        /// </summary>
        /// <param name="netSublayer">Sublayer</param>
        /// <returns></returns>
        NetSessionServer CreateSession(INetSublayer netSublayer);

        /// <summary>
        /// Create a network element on the server for the stream.
        /// </summary>
        /// <param name="name">Name of this network element</param>
        /// <param name="assetId">Asset id related to this element</param>
        /// <param name="authorId">Author id of this element; 0 is server</param>
        /// <param name="isEnabled">If true, this element will be immediately streamed</param>
        /// <param name="sibling">Handy sibling element reference; for elements that are related</param>
        /// <returns></returns>
        INetElement CreateElement(string name, uint assetId, uint authorId = 0, bool isEnabled = true, INetElement sibling = null);

        /// <summary>
        /// Find a session given an id.
        /// </summary>
        /// <param name="id">UInt</param>
        /// <param name="session">Target session by id</param>
        /// <returns></returns>
        bool FindSession(uint id, out INetSession session);
    }
}
