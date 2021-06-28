// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network int field.
    /// </summary>
    public class NetFieldInt : NetField, INetFieldInt
    {
        internal Action<int> Receive { get; set; }

        private readonly NetFieldBuffer<int> _buffer;

        /// <inheritdoc />
        public NetFieldInt(byte index, string name, bool isReliable, Action<int> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Int);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<int>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldInt(NetFieldDesc description, Action<int> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<int>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(int value)
        {
            _buffer.Add(value);
            TxQueue.WriteInt(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteInt(value);
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
                _buffer.Add(rxQueue.ReadInt());
            }
        }
    }
}
