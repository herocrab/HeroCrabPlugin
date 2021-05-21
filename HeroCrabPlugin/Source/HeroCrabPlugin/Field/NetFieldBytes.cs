using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network bytes field.
    /// </summary>
    public class NetFieldBytes: NetField, INetFieldBytes
    {
        internal Action<byte[]> Receive { get; set; }

        private readonly NetFieldBuffer<byte[]> _buffer;

        /// <inheritdoc />
        public NetFieldBytes(byte index, string name, bool isReliable, Action<byte[]> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.ByteArray);

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<byte[]>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldBytes(NetFieldDesc description, Action<byte[]> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<byte[]>(bufferSize);
        }

        /// <inheritdoc />
        public void Set(byte[] value)
        {
            _buffer.Add(value);
            TxQueue.WriteBytes(value);
            IsUpdated = true;
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
