// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network field description.
    /// </summary>
    public class NetFieldDesc : NetObject
    {
        /// <summary>
        /// Network field index value.
        /// </summary>
        public byte Index { get; }

        /// <summary>
        /// Network field name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// IsReliable is true if this field is to be sent reliably.
        /// </summary>
        public bool IsReliable { get; }

        /// <summary>
        /// Network field type code indicates value type on creation.
        /// </summary>
        public TypeCode Type { get; }

        private readonly NetByteQueue _byteQueue;

        /// <summary>
        /// Network field type code enum.
        /// </summary>
        public enum TypeCode : byte
        {
            #pragma warning disable 1591
            Byte,
            ByteArray,
            Float,
            Int,
            Long,
            String,
            UInt,
            UShort,
            Vector2,
            Vector3,
            Vector4,
            Quaternion,
            Bool,
            #pragma warning restore 1591
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Serialize this network field description.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            return _byteQueue.ToBytes();
        }

        /// <summary>
        /// Deserialize this network field description given the receive queue.
        /// </summary>
        /// <param name="rxQueue"></param>
        /// <returns></returns>
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
