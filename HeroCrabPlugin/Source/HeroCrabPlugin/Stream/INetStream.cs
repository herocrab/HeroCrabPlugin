/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */

using HeroCrabPlugin.Sublayer;

// ReSharper disable UnusedMemberInSuper.Global

namespace HeroCrabPlugin.Stream
{
    /// <summary>
    /// Network stream contains elements and sessions.
    /// </summary>
    public interface INetStream
    {
        /// <summary>
        /// Event is invoked when a session is connected.
        /// </summary>
        event NetStream.SessionConnectedHandler SessionConnected;

        /// <summary>
        /// Event is invoked when a session is disconnected.
        /// </summary>
        event NetStream.SessionDisconnectedHandler SessionDisconnected;

        /// <summary>
        /// Element created event handler.
        /// </summary>
        event NetStream.ElementCreatedHandler ElementCreated;

        /// <summary>
        /// Element deleted event handler.
        /// </summary>
        event NetStream.ElementDeletedHandler ElementDeleted;

        /// <summary>
        /// Count of elements in the stream.
        /// </summary>
        int ElementCount { get; }

        /// <summary>
        /// Count of sessions in the stream.
        /// </summary>
        int SessionCount { get; }

        /// <summary>
        /// Delete a session from the stream given the sublayer.
        /// </summary>
        /// <param name="sublayer">Sublayer of session to be deleted</param>
        void DeleteSession(INetSublayer sublayer);

        /// <summary>
        /// Process the network stream.
        /// </summary>
        /// <param name="time">Game time</param>
        void Process(float time);

        /// <summary>
        /// Clear the network stream.
        /// </summary>
        void Clear();

        /// <summary>
        /// Kick all connected sessions.
        /// </summary>
        void KickAll();
    }
}
