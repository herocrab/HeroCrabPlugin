// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network byte field.
    /// </summary>
    public class NetFieldByte : NetField, INetField<byte>, INetFieldReceiver<byte>
    {
        /// <inheritdoc />
        public Action<byte> Receive { get; set; }

        private readonly NetFieldBuffer<byte> _buffer;

        /// <inheritdoc />
        public NetFieldByte(byte index, string name, bool isReliable, Action<byte> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Byte);
            _buffer = new NetFieldBuffer<byte>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldByte(NetFieldDesc description, Action<byte> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<byte>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(byte value)
        {
            _buffer.Add(value);
            TxQueue.WriteByte(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteByte(value);
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
                _buffer.Add(rxQueue.ReadByte());
            }
        }
    }
}
