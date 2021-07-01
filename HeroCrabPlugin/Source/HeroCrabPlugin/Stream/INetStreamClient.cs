// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;

// ReSharper disable UnusedMember.Global

namespace HeroCrabPlugin.Stream
{
    /// <summary>
    /// Client stream interface, used as interface segregation between super-layer and sub-layer.
    /// </summary>
    public interface INetStreamClient : INetStream
    {
        /// <summary>
        /// Create a session, called by sublayer upon connecting to create higher-level session.
        /// </summary>
        /// <param name="netSublayer">Sublayer to create as a session</param>
        /// <returns></returns>
        NetSessionClient CreateSession(INetSublayer netSublayer);

        /// <summary>
        /// Find a session, used by game client to retrieve local session.
        /// </summary>
        /// <param name="session">Client session</param>
        /// <returns></returns>
        bool FindSession(out INetSession session);
    }
}
