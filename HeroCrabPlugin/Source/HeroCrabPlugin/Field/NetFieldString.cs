using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network string field.
    /// </summary>
    public class NetFieldString : NetField, INetFieldString
    {
        internal Action<string> Receive { get; set; }

        private readonly NetFieldBuffer<string> _buffer;

        /// <inheritdoc />
        public NetFieldString(byte index, string name, bool isReliable, Action<string> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.String);

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<string>(bufferSize);
        }

        /// <inheritdoc />
        public NetFieldString(NetFieldDesc description, Action<string> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<string>(bufferSize);
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
