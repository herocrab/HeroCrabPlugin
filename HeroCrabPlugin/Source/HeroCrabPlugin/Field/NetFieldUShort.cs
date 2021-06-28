// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network ushort field.
    /// </summary>
    public class NetFieldUShort : NetField, INetFieldUShort
    {
        internal Action<ushort> Receive { get; set; }

        private readonly NetFieldBuffer<ushort> _buffer;

        /// <inheritdoc />
        public NetFieldUShort(byte index, string name, bool isReliable, Action<ushort> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.UShort);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<ushort>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldUShort(NetFieldDesc description, Action<ushort> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<ushort>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(ushort value)
        {
            _buffer.Add(value);
            TxQueue.WriteUShort(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteUShort(value);
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
                _buffer.Add(rxQueue.ReadUShort());
            }
        }
    }
}
