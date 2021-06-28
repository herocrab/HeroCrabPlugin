// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network uint field.
    /// </summary>
    public class NetFieldUInt: NetField, INetField<uint>
    {
        internal Action<uint> Receive { get; set; }

        private readonly NetFieldBuffer<uint> _buffer;

        /// <inheritdoc />
        public NetFieldUInt(byte index, string name, bool isReliable, Action<uint> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.UInt);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<uint>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldUInt(NetFieldDesc description, Action<uint> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<uint>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(uint value)
        {
            _buffer.Add(value);
            TxQueue.WriteUInt(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteUInt(value);
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
                _buffer.Add(rxQueue.ReadUInt());
            }
        }
    }
}
