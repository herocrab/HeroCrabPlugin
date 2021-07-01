// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Sublayer;

namespace HeroCrabPlugin.Session
{
    /// <inheritdoc />
    public class NetSessionServer : NetSession
    {
        private readonly SortedDictionary<uint, NetElement> _candidateElements;
        private readonly SortedDictionary<uint, NetElement> _createdElements;
        private readonly SortedDictionary<uint, NetElement> _sessionElements;
        private readonly SortedDictionary<uint, byte> _applyInputCounter;

        private readonly byte _maximumApplyPerElement;
        private readonly byte _maximumPacketRate;

        private readonly SortedDictionary<uint, List<NetElement>> _sendElements;
        private readonly SortedDictionary<uint, List<NetElement>> _excludeElements;

        private float _samplingStart;
        private float _nextSamplingStart;
        private float _packetRate;

        private const byte MinimumTxLength = 15;
        private const ushort MaximumRxLength = 1456;
        private const byte MaximumPpsPadding = 5;

        /// <inheritdoc />
        public NetSessionServer(INetSublayer sublayer, SortedDictionary<uint, List<NetElement>> sendElements,
            SortedDictionary<uint, List<NetElement>> excludeElements) : base(sublayer)
        {
            sublayer.ReceiveIdCallback = OnReceiveId;
            _sendElements = sendElements;
            _excludeElements = excludeElements;

            _candidateElements = new SortedDictionary<uint, NetElement>();
            _createdElements = new SortedDictionary<uint, NetElement>();
            _sessionElements = new SortedDictionary<uint, NetElement>();
            _applyInputCounter = new SortedDictionary<uint, byte>();

            _maximumApplyPerElement = NetSettings.ClientBufferDepth;
            _maximumPacketRate = (byte)(NetSettings.ClientPps + MaximumPpsPadding);
        }

        /// <summary>
        /// Process the network session.
        /// </summary>
        /// <param name="time"></param>
        public void Process(float time)
        {
            ProcessPacketRate(time);
        }

        /// <inheritdoc />
        public override void Send(float time)
        {
            IsPacketReliable = false;
            TxQueue.Clear();

            IdentifyCandidates();
            WriteDelete();
            WriteCreate();
            WriteModify();
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
            if (packet.Length >= MaximumRxLength) {
                Disconnect();
                return;
            }

            _packetRate++;
            RxCount++;
            RxQueue.Clear();
            RxQueue.WriteRaw(packet);

            if (packet.Length < 1) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Zero byte packet received by session.");
                return;
            }

            ApplyInput();
        }

        private void ProcessPacketRate(float time)
        {
            if (_samplingStart == 0) {
                _samplingStart = time;
                _nextSamplingStart = time + 1;
            }

            if (time <= _nextSamplingStart) {
                return;
            }

            if (_packetRate >= _maximumPacketRate) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, $"[ERROR] Client has exceeded packet rate " +
                                                                       $"of {_maximumPacketRate + MaximumPpsPadding} pps.");
                Disconnect();
                return;
            }

            _packetRate = 0;
            _samplingStart = time;
            _nextSamplingStart = time + 1;
        }

        private void IdentifyCandidates()
        {
            if (!_sendElements.ContainsKey(Id)) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this, "Optimized Elements did not contain session ID index.");
                throw new IndexOutOfRangeException("Optimized Elements did not contain session ID index.");
            }

            _candidateElements.Clear();

            // Merge in and filter "all recipient" elements
            foreach (var element in _sendElements[0].Where(element => element.Filter.Contains(StreamGroup))) {
                _candidateElements.Add(element.Description.Id, element);
            }

            // Merge in and filter "single recipient" elements
            foreach (var element in _sendElements[Id].Where(element => element.Filter.Contains(StreamGroup))) {
                _candidateElements.Add(element.Description.Id, element);
            }

            // Check for excluded entries
            if (!_excludeElements.ContainsKey(Id)) {
                return;
            }

            // Filter out excluded entries, this reduces iteration in NetStreamServer
            foreach (var element in _excludeElements[Id].Where(element => _candidateElements.ContainsKey(element.Description.Id))) {
                _candidateElements.Remove(element.Description.Id);
            }
        }

        private void WriteDelete()
        {
            TxQueue.WriteByte((byte)InnerTypeCode.Delete);

            var deletedElements = _sessionElements.Values.Where(
                element => !_candidateElements.ContainsKey(element.Description.Id)).ToArray();

            foreach (var element in deletedElements) {
                _sessionElements.Remove(element.Description.Id);
            }

            TxQueue.WriteUInt((uint)deletedElements.Length);
            foreach (var element in deletedElements) {
                IsPacketReliable = true;
                TxQueue.WriteUInt(element.Description.Id);
            }
        }

        private void WriteCreate()
        {
            _createdElements.Clear();

            TxQueue.WriteByte((byte)InnerTypeCode.Create);

            var createdElements = _candidateElements.Values.Where(
                element => !_sessionElements.ContainsKey(element.Description.Id)).ToArray();

            foreach (var element in createdElements) {
                _createdElements.Add(element.Description.Id, element);
                _sessionElements.Add(element.Description.Id, element);
            }

            TxQueue.WriteUInt((uint)createdElements.Length);
            foreach (var element in createdElements) {
                IsPacketReliable = true;
                TxQueue.WriteRaw(element.Description.Serialize());
            }
        }

        private void WriteModify()
        {
            TxQueue.WriteByte((byte)InnerTypeCode.Modify);

            var modifiedElements = _sessionElements.Values.Where(
                element => element.IsUpdated && !_createdElements.ContainsKey(element.Description.Id)).ToArray();

            TxQueue.WriteUInt((uint)(modifiedElements.Length + _createdElements.Count));

            foreach (var element in _createdElements.Values) {
                if (element.IsReliable) {
                    IsPacketReliable = true;
                }

                TxQueue.WriteRaw(element.SerializeLast());
            }

            foreach (var element in modifiedElements) {
                if (element.IsReliable) {
                    IsPacketReliable = true;
                }

                TxQueue.WriteRaw(element.Serialize());
            }
        }

        private void ApplyInput()
        {
            var typeCode = RxQueue.ReadByte();

            if (typeCode != (byte)InnerTypeCode.Input) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Expected type code: Input, but not present.");
                Disconnect();
                return;
            }

            try {
                _applyInputCounter.Clear();
                var inputCount = RxQueue.ReadInt();
                for (var i = 0; i < inputCount; i++) {
                    var index = RxQueue.ReadUInt();
                    if (_candidateElements.ContainsKey(index) && _candidateElements[index].Description.AuthorId == Id) {
                        if (!_applyInputCounter.ContainsKey(index)) {
                            _applyInputCounter.Add(index, 1);
                        }
                        else {
                            _applyInputCounter[index]++;
                        }

                        if (_applyInputCounter[index] > _maximumApplyPerElement) {
                            Disconnect();
                            return;
                        }

                        _candidateElements[index].Apply(RxQueue);
                    }
                    else {
                        throw new InvalidDataException("[ERROR] Invalid Input element index or is not from this author.");
                    }
                }
            }
            catch(Exception) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, "[ERROR] Client sent malformed input.");
                Disconnect();
            }
        }
    }
}
