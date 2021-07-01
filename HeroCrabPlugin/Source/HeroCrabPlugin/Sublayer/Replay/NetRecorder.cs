// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using FlaxEngine;
using HeroCrabPlugin.Core;
// ReSharper disable MemberCanBePrivate.Global

namespace HeroCrabPlugin.Sublayer.Replay
{
    /// <summary>
    /// Network recorder for replay system.
    /// </summary>
    public class NetRecorder : NetObject, INetSublayer
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
        public byte[] Bytes => _bytes.ToBytes();

        /// <summary>
        /// Returns true if the network recorder is recording.
        /// </summary>
        public bool IsRecording { get; private set; }

        private readonly SortedDictionary<float, byte[]> _data;
        private readonly NetByteQueue _bytes;

        /// <summary>
        /// Network recorder for replay system.
        /// </summary>
        public NetRecorder()
        {
            _data = new SortedDictionary<float, byte[]>();
            _bytes = new NetByteQueue();
        }

        /// <summary>
        /// Start the network recorder.
        /// </summary>
        public void Start()
        {
            if (IsRecording) {
                NetLogger.Write(NetLogger.LoggingGroup.Error, this, "ERROR: Recorder attempted to start but is already running.");
                return;
            }

            _bytes.Clear();
            _data.Clear();
            IsRecording = true;
        }

        /// <summary>
        /// Stop the network recorder.
        /// </summary>
        /// <returns>Recorded bytes</returns>
        public void Stop()
        {
            IsRecording = false;

            _bytes.Clear();
            _bytes.WriteInt(_data.Count);

                foreach (var entry in _data) {
                    _bytes.WriteFloat(entry.Key);
                    _bytes.WriteBytes(entry.Value);
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

            if (!_data.ContainsKey(time)) {
                _data.Add(time, packet);
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
