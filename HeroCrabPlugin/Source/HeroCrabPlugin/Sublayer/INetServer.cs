// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer
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
