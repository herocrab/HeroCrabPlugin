// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network long field.
    /// </summary>
    public class NetFieldLong : NetField, INetField<long>, INetFieldReceiver<long>
    {
        /// <inheritdoc />
        public Action<long> Receive { get; set; }

        private readonly NetFieldBuffer<long> _buffer;

        /// <inheritdoc />
        public NetFieldLong(byte index, string name, bool isReliable, Action<long> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Long);
            _buffer = new NetFieldBuffer<long>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldLong(NetFieldDesc description, Action<long> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<long>(BufferSize);
            Receive = callback;
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
        public void SetLastValue(long value)
        {
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
