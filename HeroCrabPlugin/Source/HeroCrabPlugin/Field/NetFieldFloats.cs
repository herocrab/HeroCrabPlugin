// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.


using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network Vector2 field.
    /// </summary>
    public class NetFieldFloats : NetField, INetField<float[]>, INetFieldReceiver<float[]>
    {
        /// <inheritdoc />
        public Action<float[]> Receive { get; set; }

        private readonly NetFieldBuffer<float[]> _buffer;

        /// <inheritdoc />
        public NetFieldFloats(byte index, string name, bool isReliable, Action<float[]> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Floats);
            _buffer = new NetFieldBuffer<float[]>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldFloats(NetFieldDesc description, Action<float[]> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<float[]>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(float[] value)
        {
            _buffer.Add(value);
            TxQueue.WriteFloats(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteFloats(value);
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
                _buffer.Add(rxQueue.ReadFloats());
            }
        }
    }
}
