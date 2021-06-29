// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Field
{
    /// <inheritdoc />
    public abstract class NetField : NetObject
    {
        /// <summary>
        /// Network field description.
        /// </summary>
        public NetFieldDesc Description { get; protected set; }

        /// <summary>
        /// IsReliable is true if this field is set to be sent reliably.
        /// </summary>
        public bool IsReliable { get; }

        /// <summary>
        /// IsUpdated is true if this field has been updated and is queued for transmission.
        /// </summary>
        public bool IsUpdated { get; protected set; }

        /// <summary>
        /// Transmit queue.
        /// </summary>
        protected readonly NetByteQueue TxQueue;

        /// <summary>
        /// Queue containing the last known value for this field.
        /// </summary>
        protected readonly NetByteQueue LastQueue;

        /// <summary>
        /// Field buffer size.
        /// </summary>
        protected readonly byte BufferSize;

        private readonly NetByteQueue _serializeQueue;

        private const int MaxFieldDepth = 256;

        /// <inheritdoc />
        protected NetField(bool isReliable)
        {
            IsReliable = isReliable;
            BufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;

            TxQueue = new NetByteQueue();
            LastQueue = new NetByteQueue();
            _serializeQueue = new NetByteQueue();
        }

        /// <inheritdoc />
        protected NetField(NetFieldDesc description)
        {
            Description = description;
            IsReliable = description.IsReliable;
            BufferSize = IsReliable ? NetSettings.ReliableBufferDepth : NetSettings.UnreliableBufferDepth;

            TxQueue = new NetByteQueue();
            LastQueue = new NetByteQueue();
            _serializeQueue = new NetByteQueue();
        }

        /// <summary>
        /// Clear the net field transmit queue and set IsUpdated to false.
        /// </summary>
        public void Clear()
        {
            TxQueue.Clear();
            _serializeQueue.Clear();

            IsUpdated = false;
        }

        /// <summary>
        /// Process this network field.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Serialize network field for transmission.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
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

        /// <summary>
        /// Serialize the last known value for this field.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Deserialize a byte queue into this network field.
        /// </summary>
        /// <param name="rxQueue"></param>
        public abstract void Deserialize(NetByteQueue rxQueue);

        /// <summary>
        /// Reset this field by clearing the transmit queue and setting IsUpdated to false.
        /// </summary>
        public void Reset()
        {
            IsUpdated = false;
            TxQueue.Clear();
        }
    }
}
