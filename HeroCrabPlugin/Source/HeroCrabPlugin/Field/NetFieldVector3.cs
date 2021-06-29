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
    public class NetFieldVector3 : NetField, INetField<Vector3>
    {
        internal Action<Vector3> Receive { get; set; }

        private readonly NetFieldBuffer<Vector3> _buffer;

        /// <inheritdoc />
        public NetFieldVector3(byte index, string name, bool isReliable, Action<Vector3> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Vector3);
            _buffer = new NetFieldBuffer<Vector3>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldVector3(NetFieldDesc description, Action<Vector3> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<Vector3>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(Vector3 value)
        {
            _buffer.Add(value);
            TxQueue.WriteVector3(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteVector3(value);
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
                _buffer.Add(rxQueue.ReadVector3());
            }
        }
    }
}
