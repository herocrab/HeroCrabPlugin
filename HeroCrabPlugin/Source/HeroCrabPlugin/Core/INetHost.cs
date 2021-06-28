// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Sublayer.Udp;
// ReSharper disable UnusedMemberInSuper.Global

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network host.
    /// </summary>
    public interface INetHost : IDisposable
    {
        /// <summary>
        /// Logger event.
        /// </summary>
        event NetHost.NetLogWriteHandler LogWrite;

        /// <summary>
        /// Start a network host.
        /// </summary>
        /// <param name="ipAddress">For a server this is the listener IP; for a client this is the destination.</param>
        /// <param name="port">For a server this is the listener port; for a client this is the destination port.</param>
        void Start(string ipAddress, ushort port);

        /// <summary>
        /// Stop a network host.
        /// </summary>
        void Stop();

        /// <summary>
        /// Process a network host; this is required every game tick.
        /// </summary>
        /// <param name="time"></param>
        void Process(float time);
    }
}
