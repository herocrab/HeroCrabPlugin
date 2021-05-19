using System;
// ReSharper disable once CheckNamespace


public class NetFieldBytes: NetField, INetFieldBytes
{
    internal Action<byte[]> Receive { get; set; }

    private readonly NetFieldBuffer<byte[]> _buffer;

    public NetFieldBytes(byte index, string name, bool isReliable, Action<byte[]> callback)
    {
        IsReliable = isReliable;
        Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.ByteArray);

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<byte[]>(bufferSize);
    }

    public NetFieldBytes(NetFieldDesc description, Action<byte[]> callback)
    {
        IsReliable = description.IsReliable;
        Description = description;

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<byte[]>(bufferSize);
    }

    public void Set(byte[] value)
    {
        _buffer.Add(value);
        TxQueue.WriteBytes(value);
        IsUpdated = true;
    }

    public override void Process()
    {
        if (_buffer.Any()) {
            Receive?.Invoke(_buffer.Read());
        }
    }

    public override void Deserialize(NetByteQueue rxQueue)
    {
        var count = rxQueue.ReadByte();

        for (var i = 0; i < count; i++) {
            _buffer.Add(rxQueue.ReadBytes());
        }
    }
}
