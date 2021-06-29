// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using FlaxEngine;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network Vector2 field.
    /// </summary>
    public class NetFieldQuaternion : NetField, INetField<Quaternion>, INetFieldReceiver<Quaternion>
    {
        /// <inheritdoc />
        public Action<Quaternion> Receive { get; set; }

        private readonly NetFieldBuffer<Quaternion> _buffer;

        /// <inheritdoc />
        public NetFieldQuaternion(byte index, string name, bool isReliable, Action<Quaternion> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Quaternion);
            _buffer = new NetFieldBuffer<Quaternion>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldQuaternion(NetFieldDesc description, Action<Quaternion> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<Quaternion>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(Quaternion value)
        {
            _buffer.Add(value);
            TxQueue.WriteQuaternion(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteQuaternion(value);
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
                _buffer.Add(rxQueue.ReadQuaternion());
            }
        }
    }
}
