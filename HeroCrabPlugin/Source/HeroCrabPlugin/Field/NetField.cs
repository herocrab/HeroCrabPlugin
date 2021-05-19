using System;
// ReSharper disable once CheckNamespace


public abstract class NetField : NetObject
    {
        public NetFieldDesc Description { get; protected set; }
        public bool IsReliable { get; protected set; }
        public bool IsUpdated { get; protected set; }

        protected readonly NetByteQueue TxQueue;
        protected readonly NetByteQueue LastQueue;
        private readonly NetByteQueue _serializeQueue;

        private const int MaxFieldDepth = 256;

        protected NetField()
        {
            TxQueue = new NetByteQueue();
            LastQueue = new NetByteQueue();
            _serializeQueue = new NetByteQueue();
        }

        public void Clear()
        {
            TxQueue.Clear();
            _serializeQueue.Clear();

            IsUpdated = false;
        }

        public abstract void Process();

        public byte[] Serialize()
        {
            _serializeQueue.Clear();

            if (TxQueue.Depth <= 0) {
                _serializeQueue.WriteByte(0);
                return _serializeQueue.ToBytes();
            }

            if (TxQueue.Depth >= MaxFieldDepth) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, $"[ERROR] TxQueue depth is greater than {MaxFieldDepth}, you cannot serialize this many fields");
                throw new NotSupportedException($"[ERROR] TxQueue depth is greater than {MaxFieldDepth}, you cannot serialize this many fields");
            }

            _serializeQueue.WriteByte(Convert.ToByte(TxQueue.Depth)); // Write the depth -> how many of this type
            _serializeQueue.WriteRaw(TxQueue.ToBytes()); // Write the bytes without a length field
            return _serializeQueue.ToBytes();
        }

        public byte[] SerializeLast()
        {
            _serializeQueue.Clear();

            if (LastQueue.Depth <= 0) {
                _serializeQueue.WriteByte(0);
                return _serializeQueue.ToBytes();
            }

            _serializeQueue.WriteByte(1);
            _serializeQueue.WriteRaw(LastQueue.ToBytes());
            return _serializeQueue.ToBytes();
        }

        public abstract void Deserialize(NetByteQueue rxQueue);

        public void Reset()
        {
            IsUpdated = false;
            TxQueue.Clear();
        }
    }
