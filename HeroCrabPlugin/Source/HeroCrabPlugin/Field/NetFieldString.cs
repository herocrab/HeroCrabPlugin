using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    public class NetFieldString : NetField, INetFieldString
    {
        internal Action<string> Receive { get; set; }

        private readonly NetFieldBuffer<string> _buffer;

        public NetFieldString(byte index, string name, bool isReliable, Action<string> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.String);

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<string>(bufferSize);
        }

        public NetFieldString(NetFieldDesc description, Action<string> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<string>(bufferSize);
        }

        public void Set(string value)
        {
            _buffer.Add(value);
            TxQueue.WriteString(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteString(value);
        }

        public override void Process()
        {
            // Only process one buffered item per tick
            if (_buffer.Any()) {
                Receive?.Invoke(_buffer.Read());
            }
        }

        public override void Deserialize(NetByteQueue rxQueue)
        {
            var count = rxQueue.ReadByte();

            for (var i = 0; i < count; i++) {
                _buffer.Add(rxQueue.ReadString());
            }
        }
    }
}
