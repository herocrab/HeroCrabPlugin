// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer.Replay
{
    /// <summary>
    /// Network replay for recorded games.
    /// </summary>
    public class NetReplay : NetObject, INetSublayer, INetReplay
    {
        /// <inheritdoc />
        public uint Id { get; set; }

        /// <inheritdoc />
        public string Ip { get; }

        /// <inheritdoc />
        public uint RoundTripTime { get; } = 0;

        /// <inheritdoc />
        public Action<byte[]> ReceiveDataCallback { get; set; }

        /// <inheritdoc />
        public Action<uint> ReceiveIdCallback { get; set; }

        /// <inheritdoc />
        public INetStreamClient Stream => _stream;

        /// <inheritdoc />
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// True when unit testing, false when using stream.
        /// </summary>
        public bool DisableStream { get; set; }

        private float _startTime;

        private readonly NetStreamClient _stream;
        private readonly NetByteQueue _netByteQueue;
        private readonly SortedDictionary<float, byte[]> _replayData;
        private readonly SortedDictionary<float, byte[]> _dataChunk;

        private const int DataChunkSize = 10;


        /// <summary>
        /// Create a new network replay player.
        /// </summary>
        /// <returns></returns>
        public static INetReplay Create()
        {
            return new NetReplay();
        }

        private NetReplay()
        {
            Ip = "0.0.0.0";
            _stream = new NetStreamClient();
            _netByteQueue = new NetByteQueue();
            _replayData = new SortedDictionary<float, byte[]>();
            _dataChunk = new SortedDictionary<float, byte[]>();
        }

        /// <inheritdoc />
        public void Process(float time)
        {
            if (!IsPlaying) {
                return;
            }

            _dataChunk.Clear();
            var replayTime = time - _startTime;

            for (int i = 0; i < DataChunkSize; i++) {
                if (_replayData.Count == 0) {
                    continue;
                }

                // Evaluate the first chunk
                if (_replayData.ElementAt(0).Key > replayTime) {
                    continue;
                }
                _dataChunk.Add(_replayData.ElementAt(0).Key, _replayData.ElementAt(0).Value);
                _replayData.Remove(_dataChunk.ElementAt(0).Key);
            }

            foreach (var packet in _dataChunk.Values) {
                ReceiveDataCallback?.Invoke(packet);
            }
            _stream.Process(time);
        }

        /// <inheritdoc />
        public void Play(float time, byte[] bytes)
        {
            if (IsPlaying) {
                return;
            }

            UnreelBytes(bytes);
            IsPlaying = true;
            _startTime = time;

            _stream.KickAll();
            _stream.Clear();

            if (!DisableStream) {
                _stream.CreateSession(this);
            }

            ReceiveIdCallback?.Invoke(0);
        }

        /// <inheritdoc />
        public void Stop()
        {
            IsPlaying = false;
        }

        /// <inheritdoc />
        public void Disconnect() => Stop();

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public void Send(float time, byte[] packet, bool isReliable) {}

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public void SendId(uint id) {}

        private void UnreelBytes(byte[] bytes)
        {
            _netByteQueue.Clear();
            _netByteQueue.WriteRaw(bytes);

            _replayData.Clear();
            try {
                var count = _netByteQueue.ReadInt();
                for (int i = 0; i < count; i++) {
                    var time = _netByteQueue.ReadFloat();
                    var data = _netByteQueue.ReadBytes();
                    _replayData.Add(time, data);
                }
            } catch {
                    NetLogger.Write(NetLogger.LoggingGroup.Error, this,
                        $"ERROR: There was an error unreeling the replay.");
                    _replayData.Clear();
            }
        }
    }
}

