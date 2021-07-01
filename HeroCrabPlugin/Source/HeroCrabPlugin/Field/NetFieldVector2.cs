// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using FlaxEngine;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network Vector2 field.
    /// </summary>
    public class NetFieldVector2 : NetField, INetField<Vector2>, INetFieldReceiver<Vector2>
    {
        /// <inheritdoc />
        public Action<Vector2> Receive { get; set; }

        private readonly NetFieldBuffer<Vector2> _buffer;

        /// <inheritdoc />
        public NetFieldVector2(byte index, string name, bool isReliable, Action<Vector2> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Vector2);
            _buffer = new NetFieldBuffer<Vector2>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldVector2(NetFieldDesc description, Action<Vector2> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<Vector2>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(Vector2 value)
        {
            _buffer.Add(value);
            TxQueue.WriteVector2(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteVector2(value);
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
                _buffer.Add(rxQueue.ReadVector2());
            }
        }
    }
}
