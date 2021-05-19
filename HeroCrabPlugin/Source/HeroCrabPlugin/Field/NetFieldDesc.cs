using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    public class NetFieldDesc : NetObject
    {
        public byte Index { get; }
        public string Name { get; }
        public bool IsReliable { get; }
        public TypeCode Type { get; }

        private readonly NetByteQueue _byteQueue;

        public enum TypeCode : byte
        {
            Unknown,
            Byte,
            ByteArray,
            Float,
            Int,
            Long,
            String,
            UInt,
            UShort,
        }

        public NetFieldDesc(byte index, string name, bool isReliable, TypeCode typeCode)
        {
            Index = index;
            Name = name;
            IsReliable = isReliable;
            Type = typeCode;

            _byteQueue = new NetByteQueue();
            _byteQueue.WriteByte(Index);
            _byteQueue.WriteString(name);
            _byteQueue.WriteByte(Convert.ToByte(IsReliable));
            _byteQueue.WriteByte(Convert.ToByte(Type));
        }

        public byte[] Serialize()
        {
            return _byteQueue.ToBytes();
        }

        public static NetFieldDesc Deserialize(NetByteQueue rxQueue)
        {
            var index = rxQueue.ReadByte();
            var name = rxQueue.ReadString();
            var isReliable = Convert.ToBoolean(rxQueue.ReadByte());
            var typeCode = (TypeCode) rxQueue.ReadByte();

            return new NetFieldDesc(index, name, isReliable, typeCode);
        }
    }
}
