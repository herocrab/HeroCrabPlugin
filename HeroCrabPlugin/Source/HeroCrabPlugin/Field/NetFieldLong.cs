﻿// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network long field.
    /// </summary>
    public class NetFieldLong : NetField, INetField<long>
    {
        internal Action<long> Receive { get; set; }

        private readonly NetFieldBuffer<long> _buffer;

        /// <inheritdoc />
        public NetFieldLong(byte index, string name, bool isReliable, Action<long> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Long);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<long>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldLong(NetFieldDesc description, Action<long> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<long>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(long value)
        {
            _buffer.Add(value);
            TxQueue.WriteLong(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteLong(value);
        }

        /// <inheritdoc />
        public override void Process()
        {
            // Only process one buffered item per tick
            if (_buffer.Any()) {
                Receive?.Invoke(_buffer.Read());
            }
        }

        /// <inheritdoc />
        public override void Deserialize(NetByteQueue rxQueue)
        {
            var count = rxQueue.ReadByte();

            for (var i = 0; i < count; i++) {
                _buffer.Add(rxQueue.ReadLong());
            }
        }
    }
}
