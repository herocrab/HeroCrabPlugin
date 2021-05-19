using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    public class NetFieldLong : NetField, INetFieldLong
    {
        internal Action<long> Receive { get; set; }

        private readonly NetFieldBuffer<long> _buffer;

        public NetFieldLong(byte index, string name, bool isReliable, Action<long> callback)
        {
            IsReliable = isReliable;
            Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Long);

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<long>(bufferSize);
        }

        public NetFieldLong(NetFieldDesc description, Action<long> callback)
        {
            IsReliable = description.IsReliable;
            Description = description;

            Receive = callback;
            var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
            _buffer = new NetFieldBuffer<long>(bufferSize);
        }

        public void Set(long value)
        {
            _buffer.Add(value);
            TxQueue.WriteLong(value);
            IsUpdated = true;

            LastQueue.Clear();
            LastQueue.WriteLong(value);
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
                _buffer.Add(rxQueue.ReadLong());
            }
        }
    }
}
