using System;
using System.Linq;
using ENet;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Sublayer.Udp
{
    public class NetSublayer : NetObject, INetSublayer
    {
        public uint Id { get; set; }
        public string Ip => _peer.IP;
        public Action<byte[]> ReceiveDataCallback { get; set; }
        public Action<uint> ReceiveIdCallback { get; set; }
        public Action<Peer> DisconnectCallback { get; set; }

        private enum Channels : byte
        {
            Control,
            Data,
        }

        private Peer _peer;
        private readonly NetByteQueue _txQueue;
        private readonly NetByteQueue _rxQueue;
        private readonly byte[] _rxBuffer;
        private bool _isAssignedId;

        private const ushort MaximumPacketLength = 1456;

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

        public void Disconnect()
        {
            _peer.DisconnectNow(0);
            DisconnectCallback?.Invoke(_peer);
        }

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
            packet.CopyTo(_rxBuffer);
            ReceiveDataCallback?.Invoke(_rxBuffer.Take(packet.Length).ToArray());
            packet.Dispose();
        }

        public void SendId(uint id)
        {
            Id = id;
            _isAssignedId = true;

            _txQueue.Clear();
            _txQueue.WriteUInt(Id);

            var packet = default(Packet);
            packet.Create(_txQueue.ToBytes(), PacketFlags.Reliable | PacketFlags.Instant);
            _peer.Send((byte) Channels.Control, ref packet);
        }

        private void ReceiveId(Packet packet)
        {
            packet.CopyTo(_rxBuffer);

            _rxQueue.Clear();
            _rxQueue.WriteRaw(_rxBuffer.Take(packet.Length).ToArray());

            if (_rxQueue.Length != 4) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client received invalid session ID for assignment.");
                packet.Dispose();
                return;
            }

            var id = _rxQueue.ReadUInt();

            if (_isAssignedId && id != Id) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client attempted to change the session Id.");
                Disconnect();
                packet.Dispose();
            }

            ReceiveIdCallback?.Invoke(id);
            packet.Dispose();

            if (_isAssignedId) {
                return;
            }

            SendId(id);
        }
    }
}
