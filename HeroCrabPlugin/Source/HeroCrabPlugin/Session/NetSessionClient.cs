// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Sublayer;

// ReSharper disable NotResolvedInText

namespace HeroCrabPlugin.Session
{
    /// <summary>
    /// Network client session.
    /// </summary>
    public class NetSessionClient : NetSession
    {
        private readonly SortedDictionary<uint, NetElement> _inputElements;
        private readonly SortedDictionary<uint, NetElement> _elements;

        private const byte MinimumTxLength = 5;

        /// <inheritdoc />
        public NetSessionClient(INetSublayer sublayer, SortedDictionary<uint, NetElement> elements) : base(sublayer)
        {
            sublayer.ReceiveIdCallback = OnReceiveId;
            _inputElements = new SortedDictionary<uint, NetElement>();
            _elements = elements;
        }

        /// <inheritdoc />
        public override void Send(float time)
        {
            IsPacketReliable = false;
            TxQueue.Clear();

            WriteInput();
            SendPacket(time, TxQueue.ToBytes(), IsPacketReliable);
        }

        /// <inheritdoc />
        protected override void SendPacket(float time, byte[] packet, bool isReliable)
        {
            if (packet.Length <= MinimumTxLength) {
                return;
            }

            base.SendPacket(time, packet, isReliable);
        }

        /// <inheritdoc />
        protected override void OnReceivePacket(byte[] packet)
        {
            RxCount++;
            RxQueue.Clear();
            RxQueue.WriteRaw(packet);

            if (packet.Length < 1) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Zero byte packet received by session.");
                throw new ArgumentOutOfRangeException("[ERROR] Zero byte packet received by session.");
            }

            ApplyDelete();
            ApplyCreate();
            ApplyModify();
        }

        private void WriteInput()
        {
            TxQueue.WriteByte((byte)InnerTypeCode.Input);

            var modifiedInput = _inputElements.Values.Where(element =>
                element.IsUpdated && element.Filter.Contains(StreamGroup)).ToArray();

            TxQueue.WriteInt(modifiedInput.Length);
            foreach (var element in modifiedInput) {
                if (element.IsReliable) {
                    IsPacketReliable = true;
                }

                TxQueue.WriteRaw(element.Serialize());
            }
        }

        private void ApplyDelete()
        {
            var typeCode = RxQueue.ReadByte();

            if (typeCode != (byte)InnerTypeCode.Delete) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Expected type code: Delete, but not present.");
                throw new ArgumentOutOfRangeException();
            }

            var deleteCount = RxQueue.ReadInt();
            for (var i = 0; i < deleteCount; i++) {
                var index = RxQueue.ReadUInt();
                if (!_elements.ContainsKey(index)) {
                    DupeCount++;
                    continue;
                }

                if (_inputElements.ContainsKey(index)) {
                    _inputElements.Remove(index);
                }

                ElementDeleted?.Invoke(_elements[index]);
                _elements.Remove(index);
            }
        }

        private void ApplyCreate()
        {
            var typeCode = RxQueue.ReadByte();

            if (typeCode != (byte)InnerTypeCode.Create) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Expected type code: Create, but not present.");
                throw new ArgumentOutOfRangeException();
            }

            var createCount = RxQueue.ReadInt();

            for (var i = 0; i < createCount; i++) {
                var elementDesc = NetElementDesc.Deserialize(RxQueue);
                if (_elements.ContainsKey(elementDesc.Id)) {
                    DupeCount++;
                    continue;
                }

                var element = new NetElement(elementDesc)
                {
                    Enabled = true,
                    IsServer = false,
                    IsClient = true
                };

                // ReSharper disable once InvertIf
                if (elementDesc.AuthorId == Id && !_inputElements.ContainsKey(elementDesc.Id)) {
                    _inputElements.Add(elementDesc.Id, element);
                }

                _elements.Add(elementDesc.Id, element);
                ElementCreated?.Invoke(element);
            }
        }

        private void ApplyModify()
        {
            var typeCode = RxQueue.ReadByte();

            if (typeCode != (byte)InnerTypeCode.Modify) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Expected type code: Modify, but not present.");
                throw new ArgumentOutOfRangeException();
            }

            var modifyCount = RxQueue.ReadUInt();
            for (var i = 0; i < modifyCount; i++) {
                var index = RxQueue.ReadUInt();
                if (_elements.ContainsKey(index)) {
                    _elements[index].Apply(RxQueue);
                }
                else {
                    RxQueue.ReadBytes();
                    MissCount++;
                }
            }
        }
    }
}
