// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network ushort field.
    /// </summary>
    public class NetFieldUShort : NetField, INetField<ushort>, INetFieldReceiver<ushort>
    {
        /// <inheritdoc />
        public Action<ushort> Receive { get; set; }

        private readonly NetFieldBuffer<ushort> _buffer;

        /// <inheritdoc />
        public NetFieldUShort(byte index, string name, bool isReliable, Action<ushort> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.UShort);
            _buffer = new NetFieldBuffer<ushort>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldUShort(NetFieldDesc description, Action<ushort> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<ushort>(BufferSize);
            Receive = callback;
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
        public void SetLastValue(ushort value)
        {
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
