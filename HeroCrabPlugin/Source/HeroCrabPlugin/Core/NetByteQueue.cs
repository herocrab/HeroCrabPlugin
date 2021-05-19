using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable once CheckNamespace

public class NetByteQueue
{
    public int Length => _byteQueue.Count;
    public int Depth => _depth;

    private const int MaxBytesLength = 512;

    private readonly Queue<byte> _byteQueue;
    private int _depth;

    private readonly byte[] _longReadArray = new byte[512];
    private readonly byte[] _shortReadArray = new byte[8];
    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public NetByteQueue() {
        _byteQueue = new Queue<byte>();
    }

    public void Clear()
    {
        _byteQueue.Clear();
        _depth = 0;
    }

    public byte[] ToBytes()
    {
        return _byteQueue.ToArray();
    }

    public void WriteBytes(byte[] bytes)
    {
        if (bytes.Length > MaxBytesLength) {
            bytes = bytes.Take(MaxBytesLength).ToArray();
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
    public void WriteRaw(byte[] bytes)
    {
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public void WriteByte(byte value) {
        _byteQueue.Enqueue(value);
        _depth++;
    }

    public void WriteString(string stringValue)
    {
        if (stringValue.Length > ushort.MaxValue) {
            stringValue = stringValue.Substring(0, ushort.MaxValue);
        }

        var length = Convert.ToUInt16(stringValue.Length);

        var stringLength = BitConverter.GetBytes(length);
        foreach (var a in stringLength) {
            _byteQueue.Enqueue(a);
        }

        foreach (var c in stringValue) {
            _byteQueue.Enqueue(Convert.ToByte(c));
        }
        _depth++;
    }

    public void WriteUShort(ushort ushortValue)
    {
        var bytes = BitConverter.GetBytes(ushortValue);
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public void WriteUInt(uint uintValue)
    {
        var bytes = BitConverter.GetBytes(uintValue);
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public void WriteInt(int intValue)
    {
        var bytes = BitConverter.GetBytes(intValue);
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public void WriteLong(long longValue)
    {
        var bytes = BitConverter.GetBytes(longValue);
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public void WriteFloat(float floatValue)
    {
        var bytes = BitConverter.GetBytes(floatValue);
        foreach (var b in bytes) {
            _byteQueue.Enqueue(b);
        }
        _depth++;
    }

    public string ReadString()
    {
        var length = ReadUShort();
        _stringBuilder.Clear();

        for (var i = 0; i < length; i++) {
            _stringBuilder.Append(Convert.ToChar(_byteQueue.Dequeue()));
        }

        return _stringBuilder.ToString();
    }

    public long ReadLong()
    {
        for (var i = 0; i < 8; i++) {
            _shortReadArray[i] = _byteQueue.Dequeue();
        }

        return BitConverter.ToInt64(_shortReadArray, 0);
    }

    public byte ReadByte()
    {
        return _byteQueue.Dequeue();
    }

    public ushort ReadUShort()
    {
        for (var i = 0; i < 2; i++) {
            _shortReadArray[i] = _byteQueue.Dequeue();
        }

        return BitConverter.ToUInt16(_shortReadArray, 0);
    }

    public int ReadInt()
    {
        for (var i = 0; i < 4; i++) {
            _shortReadArray[i] = _byteQueue.Dequeue();
        }

        return BitConverter.ToInt32(_shortReadArray, 0);
    }

    public uint ReadUInt()
    {
        for (var i = 0; i < 4; i++) {
            _shortReadArray[i] = _byteQueue.Dequeue();
        }

        return BitConverter.ToUInt32(_shortReadArray, 0);
    }

    public float ReadFloat()
    {
        for (var i = 0; i < 4; i++) {
            _shortReadArray[i] = _byteQueue.Dequeue();
        }

        return BitConverter.ToSingle(_shortReadArray, 0);
    }

    public byte[] ReadBytes()
    {
        var length = ReadInt();

        if (length > MaxBytesLength) {
            length = MaxBytesLength;
        }

        for (var i = 0; i < length; i++) {
            _longReadArray[i] = _byteQueue.Dequeue();
        }

        return _longReadArray.Take(length).ToArray();
    }

    public byte PeekByte()
    {
        return _byteQueue.Peek();
    }

    public bool Any() => _byteQueue.Any();
}
