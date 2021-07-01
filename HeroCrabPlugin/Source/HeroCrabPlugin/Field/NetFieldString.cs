/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network string field.
    /// </summary>
    public class NetFieldString : NetField, INetField<string>, INetFieldReceiver<string>
    {
        /// <inheritdoc />
        public Action<string> Receive { get; set; }

        private readonly NetFieldBuffer<string> _buffer;

        /// <inheritdoc />
        public NetFieldString(byte index, string name, bool isReliable, Action<string> callback = null) : base (isReliable)
        {
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.String);
            _buffer = new NetFieldBuffer<string>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public NetFieldString(NetFieldDesc description, Action<string> callback = null) : base (description)
        {
            _buffer = new NetFieldBuffer<string>(BufferSize);
            Receive = callback;
        }

        /// <inheritdoc />
        public void Set(string value)
        {
            _buffer.Add(value);
            TxQueue.WriteString(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteString(value);
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
                _buffer.Add(rxQueue.ReadString());
            }
        }
    }
}
