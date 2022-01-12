// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network bytes field.
    /// </summary>
    public class NetFieldBytes: NetField, INetField<byte[]>, INetFieldReceiver<byte[]>
    {
        /// <inheritdoc />
        public Action<byte[]> Receive { get; set; }

        private readonly NetFieldBuffer<byte[]> _buffer;

        /// <inheritdoc />
        public NetFieldBytes(byte index, string name, bool isReliable, Action<byte[]> callback = null)  : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.ByteArray);
            _buffer = new NetFieldBuffer<byte[]>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldBytes(NetFieldDesc description, Action<byte[]> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<byte[]>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(byte[] value)
        {
            _buffer.Add(value);
            TxQueue.WriteBytes(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteBytes(value);
        }

        /// <inheritdoc />
        public override void Process()
        {
            if (_buffer.Any()) {
                Receive?.Invoke(_buffer.Read());
            }
        }

        /// <inheritdoc />
        public override void Deserialize(NetByteQueue rxQueue)
        {
            var count = rxQueue.ReadByte();

            for (var i = 0; i < count; i++) {
                _buffer.Add(rxQueue.ReadBytes());
            }
        }
    }
}
