﻿using HeroCrabPlugin.Core;
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
        /// <param name="netSublayer">INetSublayer</param>
        void DeleteSession(INetSublayer netSublayer);

        /// <summary>
        /// Process the network stream.
        /// </summary>
        /// <param name="time"></param>
        void Process(float time);

        /// <summary>
        /// Clear the network stream.
        /// </summary>
        void Clear();
    }
}
