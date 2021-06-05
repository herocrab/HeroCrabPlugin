using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network byte field.
    /// </summary>
    public class NetFieldByte : NetField, INetFieldByte
    {
        internal Action<byte> Receive { get; set; }

        private readonly NetFieldBuffer<byte> _buffer;

        /// <inheritdoc />
        public NetFieldByte(byte index, string name, bool isReliable, Action<byte> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Byte);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<byte>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldByte(NetFieldDesc description, Action<byte> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<byte>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(byte value)
        {
            _buffer.Add(value);
            TxQueue.WriteByte(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteByte(value);
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
                _buffer.Add(rxQueue.ReadByte());
            }
        }
    }
}
