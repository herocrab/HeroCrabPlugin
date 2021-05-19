using System;
// ReSharper disable once CheckNamespace

public class NetFieldInt : NetField, INetFieldInt
{
    internal Action<int> Receive { get; set; }

    private readonly NetFieldBuffer<int> _buffer;

    public NetFieldInt(byte index, string name, bool isReliable, Action<int> callback)
    {
        IsReliable = isReliable;
        Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Int);

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<int>(bufferSize);
    }

    public NetFieldInt(NetFieldDesc description, Action<int> callback)
    {
        IsReliable = description.IsReliable;
        Description = description;

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<int>(bufferSize);
    }

    public void Set(int value)
    {
        _buffer.Add(value);
        TxQueue.WriteInt(value);
        IsUpdated = true;

        LastQueue.Clear();
        LastQueue.WriteInt(value);
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
            _buffer.Add(rxQueue.ReadInt());
        }
    }
}
