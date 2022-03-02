// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;

namespace HeroCrabPlugin.Sublayer
{
    /// <summary>
    /// Network sub-layer; Used for interface segregation and modularity.
    /// </summary>
    public interface INetSublayer
    {
        /// <summary>
        /// Sub-layer id.
        /// </summary>
        uint Id { get; set; }

        /// <summary>
        /// Sub-layer remote ip address.
        /// </summary>
        string Ip { get; }

        /// <summary>
        /// Round trip time.
        /// </summary>
        uint RoundTripTime { get; }

        /// <summary>
        /// Receive data call back; this method is invoked when data is received.
        /// </summary>
        Action<byte[]> ReceiveDataCallback { get; set; }

        /// <summary>
        /// Receive id call back; this method is invoked when an id is assigned over the network.
        /// </summary>
        Action<uint> ReceiveIdCallback { get; set; }

        /// <summary>
        /// Disconnect the sub-layer.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Send a packet through the sub-layer.
        /// </summary>
        /// <param name="time">Game time this packet is sent</param>
        /// <param name="packet">Packet data in raw bytes</param>
        /// <param name="isReliable">Reliable flag; set to true for packets that require reliable delivery</param>
        void Send(float time, byte[] packet, bool isReliable);

        /// <summary>
        /// Send a session id over this sublayer; Id's are assigned to the sub-layer when a session is created.
        /// </summary>
        /// <param name="id">Session id</param>
        void SendId(uint id);
    }
}
