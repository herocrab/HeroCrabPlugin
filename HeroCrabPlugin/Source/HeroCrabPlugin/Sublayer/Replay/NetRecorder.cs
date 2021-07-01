// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using System;
using System.Collections.Generic;
using HeroCrabPlugin.Core;
// ReSharper disable MemberCanBePrivate.Global

namespace HeroCrabPlugin.Sublayer.Replay
{
    /// <summary>
    /// Network recorder for replay system.
    /// </summary>
    public class NetRecorder : NetObject, INetSublayer, INetRecorder
    {
        /// <inheritdoc />
        public uint Id { get; set; }

        /// <inheritdoc />
        public string Ip { get; }

        /// <inheritdoc />
        public Action<byte[]> ReceiveDataCallback { get; set; }

        /// <inheritdoc />
        public Action<uint> ReceiveIdCallback { get; set; }

        /// <summary>
        /// Network recorder bytes, the result of stopping a recording.
        /// </summary>
        public byte[] Bytes => _replayBytes.ToBytes();

        /// <summary>
        /// Returns true if the network recorder is recording.
        /// </summary>
        public bool IsRecording { get; private set; }

        private float _startTime;

        private readonly SortedDictionary<float, byte[]> _replayData;
        private readonly NetByteQueue _replayBytes;

        /// <summary>
        /// Network recorder for replay system.
        /// </summary>
        public NetRecorder()
        {
            Ip = "0.0.0.0";
            _replayData = new SortedDictionary<float, byte[]>();
            _replayBytes = new NetByteQueue();
        }

        /// <summary>
        /// Start the network recorder.
        /// </summary>
        public void Start(float time)
        {
            if (IsRecording) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this,
                    "ERROR: Recorder attempted to start but is already running.");
                return;
            }

            _replayBytes.Clear();
            _replayData.Clear();
            IsRecording = true;
            _startTime = time;
        }

        /// <summary>
        /// Stop the network recorder.
        /// </summary>
        /// <returns>Recorded bytes</returns>
        public void Stop()
        {
            IsRecording = false;

            _replayBytes.Clear();
            _replayBytes.WriteInt(_replayData.Count);

                foreach (var entry in _replayData) {
                    _replayBytes.WriteFloat(entry.Key);
                    _replayBytes.WriteBytes(entry.Value);
                }
        }

        /// <inheritdoc />
        public void Disconnect() => Stop();

        /// <inheritdoc />
        public void Send(float time, byte[] packet, bool _)
        {
            if (!IsRecording) {
                return;
            }

            if (!_replayData.ContainsKey(time)) {
                _replayData.Add(time - _startTime, packet);
            }
        }

        /// <inheritdoc />
        public void SendId(uint id)
        {
            Id = id;
            ReceiveIdCallback?.Invoke(id);
        }
    }
}
