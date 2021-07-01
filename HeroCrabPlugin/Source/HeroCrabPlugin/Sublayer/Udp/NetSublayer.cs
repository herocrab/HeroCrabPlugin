// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using ENet;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Crypto;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Network sublayer for UDP using XXTEA pre-shared keys with Kek and Tek.
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

        /// <summary>
        /// Disconnect callback.
        /// </summary>
        public Action<Peer> DisconnectCallback { get; set; }

        private enum Channels : byte
        {
            Control,
            Data
        }

        private Peer _peer;
        private readonly ICryptoModule _cryptoModule;
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
            _cryptoModule = new XxteaCryptoModule();
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
        public void Send(float time, byte[] data, bool isReliable)
        {
            if (data.Length > MaximumPacketLength) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this,
                    $"[ERROR] Attempted to send a packet larger than {MaximumPacketLength}.");
                return;
            }

            var encryptedData = _cryptoModule.Encrypt(data, _xxteaPskTeK);

            var packet = default(Packet);
            packet.Create(encryptedData, isReliable
                ? PacketFlags.Reliable | PacketFlags.Instant
                : PacketFlags.None | PacketFlags.Instant);

            _peer.Send((byte) Channels.Data, ref packet);
        }

        private void OnReceivePacket(Packet packet)
        {
            packet.CopyTo(_rxBuffer);

            var data = _rxBuffer.Take(packet.Length).ToArray();
            var decryptedData = _cryptoModule.Decrypt(data, _xxteaPskTeK);

            ReceiveDataCallback?.Invoke(decryptedData);
            packet.Dispose();
        }

        /// <inheritdoc />
        public void SendId(uint id)
        {
            Id = id;
            _isAssignedId = true;

            _txQueue.Clear();
            _txQueue.WriteUInt(Id);

            if (string.IsNullOrEmpty(_xxteaPskTeK)) {
                _xxteaPskTeK = Guid.NewGuid().ToString();
            }

            _txQueue.WriteString(_xxteaPskTeK);

            var data = _txQueue.ToBytes();
            var encryptedData = _cryptoModule.Encrypt(data, XxteaPskKek);

            var packet = default(Packet);
            packet.Create(encryptedData, PacketFlags.Reliable | PacketFlags.Instant);
            _peer.Send((byte) Channels.Control, ref packet);
        }

        private void ReceiveId(Packet packet)
        {
            packet.CopyTo(_rxBuffer);

            _rxQueue.Clear();
            _rxQueue.WriteRaw(_rxBuffer.Take(packet.Length).ToArray());

            var data = _rxQueue.ToBytes();
            var decryptedData = _cryptoModule.Decrypt(data, XxteaPskKek);

            if (decryptedData.Length != 42) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client received invalid session ID for assignment.");
                packet.Dispose();
                return;
            }

            _rxQueue.Clear();
            _rxQueue.WriteRaw(decryptedData);

            var id = _rxQueue.ReadUInt();
            var tek = _rxQueue.ReadString();

            if (_isAssignedId && id != Id) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    "[ERROR] Client attempted to modify the assigned session Id.");
                Disconnect();
                packet.Dispose();
            }

            if (_isAssignedId && tek != _xxteaPskTeK) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this,
                    "[ERROR] Traffic encrypting key mis-match.");
                Disconnect();
                packet.Dispose();
            }

            ReceiveIdCallback?.Invoke(id);
            packet.Dispose();

            if (_isAssignedId) {
                return;
            }

            _xxteaPskTeK = tek;
            SendId(id);
        }
    }
}
