// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network float field.
    /// </summary>
    public class NetFieldFloat: NetField, INetField<float>, INetFieldReceiver<float>
    {
        /// <inheritdoc />
        public Action<float> Receive { get; set; }

        private readonly NetFieldBuffer<float> _buffer;

        /// <inheritdoc />
        public NetFieldFloat(byte index, string name, bool isReliable, Action<float> callback = null)  : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Float);
            _buffer = new NetFieldBuffer<float>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldFloat(NetFieldDesc description, Action<float> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<float>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(float value)
        {
            _buffer.Add(value);
            TxQueue.WriteFloat(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteFloat(value);
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
                _buffer.Add(rxQueue.ReadFloat());
            }
        }
    }
}
