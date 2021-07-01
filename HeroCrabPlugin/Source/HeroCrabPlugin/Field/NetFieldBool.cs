// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network Vector2 field.
    /// </summary>
    public class NetFieldBool : NetField, INetField<bool>, INetFieldReceiver<bool>
    {
        /// <inheritdoc />
        public Action<bool> Receive { get; set; }

        private readonly NetFieldBuffer<bool> _buffer;

        /// <inheritdoc />
        public NetFieldBool(byte index, string name, bool isReliable, Action<bool> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Bool);
            _buffer = new NetFieldBuffer<bool>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldBool(NetFieldDesc description, Action<bool> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<bool>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(bool value)
        {
            _buffer.Add(value);
            TxQueue.WriteBool(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteBool(value);
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
                _buffer.Add(rxQueue.ReadBool());
            }
        }
    }
}
