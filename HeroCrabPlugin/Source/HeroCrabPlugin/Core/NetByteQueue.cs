/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
 Distributed under the MIT license. See the LICENSE.md file in the project root for more information.*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Flexible byte queue serializer for common value types.
    /// </summary>
    public class NetByteQueue
    {
        /// <summary>
        /// Byte length of queue (number of bytes serialized).
        /// </summary>
        public int Length => _byteQueue.Count;

        /// <summary>
        /// Depth or count of fields that have been added to this queue.
        /// </summary>
        public int Depth => _depth;

        private const int MaxBytesLength = 65535;

        private readonly Queue<byte> _byteQueue;
        private int _depth;

        private readonly byte[] _longReadArray = new byte[65535];
        private readonly byte[] _shortReadArray = new byte[8];
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// Flexible byte queue serializer for common value types.
        /// </summary>
        public NetByteQueue() {
            _byteQueue = new Queue<byte>();
        }

        /// <summary>
        /// Clear the queue of all contents.
        /// </summary>
        public void Clear()
        {
            _byteQueue.Clear();
            _depth = 0;
        }

        /// <summary>
        /// Queue contents in bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return _byteQueue.ToArray();
        }

        /// <summary>
        /// Write an array of float values to the queue, maximum length of 255.
        /// </summary>
        /// <param name="floatArray">Array of floats, less than 255 in length.</param>
        public void WriteFloats(float[] floatArray)
        {
            var length = floatArray.Length;
            if (length > byte.MaxValue) {
                throw new ArgumentOutOfRangeException($"[ERROR]: Array of floats greater than 255.");
            }

            _byteQueue.Enqueue((byte)length);
            foreach (var floatValue in floatArray) {
                var bytes = BitConverter.GetBytes(floatValue);
                foreach (var b in bytes) {
                    _byteQueue.Enqueue(b);
                }
            }
            _depth++;
        }

        /// <summary>
        /// Add a series of bytes to the queue to be retrieved with ReadBytes.
        /// </summary>
        /// <param name="bytes">Bytes</param>
        public void WriteBytes(byte[] bytes)
        {
            if (bytes.Length > MaxBytesLength) {
                return;
            }

            var bytesLength = BitConverter.GetBytes(bytes.Length);

            foreach (var a in bytesLength) {
                _byteQueue.Enqueue(a);
            }

            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        /// <summary>
        /// Add a series of raw bytes to this queue.
        /// </summary>
        /// <param name="bytes">Raw bytes</param>
        public void WriteRaw(byte[] bytes)
        {
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Add a byte to this queue.
        /// </summary>
        /// <param name="value">Single byte</param>
        public void WriteByte(byte value) {
            _byteQueue.Enqueue(value);
            _depth++;
        }

        /// <summary>
        /// Add a string to this queue.
        /// </summary>
        /// <param name="value">String</param>
        public void WriteString(string value)
        {
            if (value == null) {
                value = string.Empty;
            }

            if (value.Length > ushort.MaxValue) {
                value = value.Substring(0, ushort.MaxValue);
            }

            var length = Convert.ToUInt16(value.Length);

            var stringLength = BitConverter.GetBytes(length);
            foreach (var a in stringLength) {
                _byteQueue.Enqueue(a);
            }

            foreach (var c in value) {
                _byteQueue.Enqueue(Convert.ToByte(c));
            }
            _depth++;
        }

        /// <summary>
        /// Add a ushort (UInt16) to this queue.
        /// </summary>
        /// <param name="value">UInt16</param>
        public void WriteUShort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Add a uint (UInt32) to this queue.
        /// </summary>
        /// <param name="value">UInt32</param>
        public void WriteUInt(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Add an int (Int32) to this queue.
        /// </summary>
        /// <param name="value">Int32</param>
        public void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Add a long (Int64) to this queue.
        /// </summary>
        /// <param name="value">Int64</param>
        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Add a float to this queue.
        /// </summary>
        /// <param name="floatValue">Float</param>
        public void WriteFloat(float floatValue)
        {
            var bytes = BitConverter.GetBytes(floatValue);
            foreach (var b in bytes) {
                _byteQueue.Enqueue(b);
            }
            _depth++;
        }

        /// <summary>
        /// Write a bool to this queue.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBool(bool value)
        {
            var boolean = (byte)(value ? 1 : 0);
            _byteQueue.Enqueue(boolean);
            _depth++;
        }

        /// <summary>
        /// Read a string from this queue.
        /// </summary>
        /// <returns>String</returns>
        public string ReadString()
        {
            var length = ReadUShort();
            _stringBuilder.Clear();

            for (var i = 0; i < length; i++) {
                _stringBuilder.Append(Convert.ToChar(_byteQueue.Dequeue()));
            }

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Read a long from this queue.
        /// </summary>
        /// <returns>Long</returns>
        public long ReadLong()
        {
            for (var i = 0; i < 8; i++) {
                _shortReadArray[i] = _byteQueue.Dequeue();
            }

            return BitConverter.ToInt64(_shortReadArray, 0);
        }

        /// <summary>
        /// Read a single byte from this queue.
        /// </summary>
        /// <returns>Byte</returns>
        public byte ReadByte()
        {
            return _byteQueue.Dequeue();
        }

        /// <summary>
        /// Read a ushort (UInt16) from this queue.
        /// </summary>
        /// <returns>UInt16</returns>
        public ushort ReadUShort()
        {
            for (var i = 0; i < 2; i++) {
                _shortReadArray[i] = _byteQueue.Dequeue();
            }

            return BitConverter.ToUInt16(_shortReadArray, 0);
        }

        /// <summary>
        /// Read a int (Int32) from this queue.
        /// </summary>
        /// <returns>Int32</returns>
        public int ReadInt()
        {
            for (var i = 0; i < 4; i++) {
                _shortReadArray[i] = _byteQueue.Dequeue();
            }

            return BitConverter.ToInt32(_shortReadArray, 0);
        }

        /// <summary>
        /// Read a uint (Int64) from this queue.
        /// </summary>
        /// <returns>Int64</returns>
        public uint ReadUInt()
        {
            for (var i = 0; i < 4; i++) {
                _shortReadArray[i] = _byteQueue.Dequeue();
            }

            return BitConverter.ToUInt32(_shortReadArray, 0);
        }

        /// <summary>
        /// Read a float from this queue.
        /// </summary>
        /// <returns>Float</returns>
        public float ReadFloat()
        {
            for (var i = 0; i < 4; i++) {
                _shortReadArray[i] = _byteQueue.Dequeue();
            }

            return BitConverter.ToSingle(_shortReadArray, 0);
        }

        /// <summary>
        /// Read a bool from this queue.
        /// </summary>
        /// <returns>Byte</returns>
        public bool ReadBool()
        {
            var value = _byteQueue.Dequeue();
            return value == 1 ? true : false;
        }

        /// <summary>
        /// Read a series of bytes from this queue, includes length.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            var length = ReadInt();

            if (length > MaxBytesLength) {
                return new byte[0];
            }

            for (var i = 0; i < length; i++) {
                _longReadArray[i] = _byteQueue.Dequeue();
            }

            return _longReadArray.Take(length).ToArray();
        }

        /// <summary>
        /// Peek the next byte.
        /// </summary>
        /// <returns>The next byte</returns>
        public byte PeekByte()
        {
            return _byteQueue.Peek();
        }

        /// <summary>
        /// Returns true if there are bytes in the queue.
        /// </summary>
        /// <returns>Bool</returns>
        public bool Any() => _byteQueue.Any();

        /// <summary>
        /// Read an array of float values from the queue.
        /// </summary>
        /// <returns>An array of floats</returns>
        public float[] ReadFloats()
        {
            var length = ReadByte();
            var floatArray = new float[length];
            for (int i = 0; i < length; i++) {
                floatArray[i] = ReadFloat();
            }
            return floatArray;
        }
    }
}
