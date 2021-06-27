using System;
using System.Linq;
using ENet;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Network sublayer for UDP.
    /// </summary>
    public class NetSublayer : NetObject, INetSublayer
    {
        /// <inheritdoc />
        public uint Id { get; set; }

        /// <inheritdoc />
        public string Ip => _peer.IP;

        /// <inheritdoc />
        public Action<byte[]> ReceiveDataCallback { get; set; }

        /// <inheritdoc />
        public Action<uint> ReceiveIdCallback { get; set; }

        /// <inheritdoc />
        public Action<Peer> DisconnectCallback { get; set; }

        private enum Channels : byte
        {
            Control,
            Data
        }

        private Peer _peer;
        private readonly NetByteQueue _txQueue;
        private readonly NetByteQueue _rxQueue;
        private readonly byte[] _rxBuffer;
        private bool _isAssignedId;
        private string _xxteaPskTeK = string.Empty;

        private const ushort MaximumPacketLength = 1456;
        private const string XxteaPskKek = "!!Xxtea_P$K_K3K";

        /// <summary>
        /// Create a sublayer (UDP) from an enet peer.
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        public static NetSublayer Create(Peer peer)
        {
            return new NetSublayer(peer);
        }

        private NetSublayer(Peer peer)
        {
            _peer = peer;
            _txQueue = new NetByteQueue();
            _rxQueue = new NetByteQueue();
            _rxBuffer = new byte[MaximumPacketLength];
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            _peer.DisconnectNow(0);
            DisconnectCallback?.Invoke(_peer);
        }

        /// <summary>
        /// Receive a packet from a host given the channel and packet.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="packet"></param>
        public void ReceivePacket(byte channel, Packet packet)
        {
            switch (channel) {
                case (byte) Channels.Control:
                    ReceiveId(packet);
                    break;
                case (byte) Channels.Data:
                    OnReceivePacket(packet);
                    break;
            }
        }

        /// <inheritdoc />
        public void Send(byte[] data, bool isReliable)
        {
            if (data.Length > MaximumPacketLength) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this,
                    $"[ERROR] Attempted to send a packet larger than {MaximumPacketLength}.");
                return;
            }

            var packet = default(Packet);
            packet.Create(data, isReliable
                ? PacketFlags.Reliable | PacketFlags.Instant
                : PacketFlags.None | PacketFlags.Instant);

            _peer.Send((byte) Channels.Data, ref packet);
        }

        private void OnReceivePacket(Packet packet)
        {
            // TODO decrypt the message with the tek

            packet.CopyTo(_rxBuffer);
            ReceiveDataCallback?.Invoke(_rxBuffer.Take(packet.Length).ToArray());
            packet.Dispose();
        }

        /// <inheritdoc />
        public void SendId(uint id)
        {
            Id = id;
            _isAssignedId = true;

            _txQueue.Clear();
            _txQueue.WriteUInt(Id);

            // TODO entry for sending and setting the GUID

            var packet = default(Packet);
            packet.Create(_txQueue.ToBytes(), PacketFlags.Reliable | PacketFlags.Instant);
            _peer.Send((byte) Channels.Control, ref packet);
        }

        private void ReceiveId(Packet packet)
        {
            packet.CopyTo(_rxBuffer);

            _rxQueue.Clear();
            _rxQueue.WriteRaw(_rxBuffer.Take(packet.Length).ToArray());

            // TODO update packet length
            if (_rxQueue.Length != 4) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client received invalid session ID for assignment.");
                packet.Dispose();
                return;
            }

            var id = _rxQueue.ReadUInt();

            if (_isAssignedId && id != Id) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client attempted to modify the assigned session Id.");
                Disconnect();
                packet.Dispose();
            }

            // TODO receive the TEK

            ReceiveIdCallback?.Invoke(id);
            packet.Dispose();

            if (_isAssignedId) {
                return;
            }

            SendId(id);
        }
    }
}
