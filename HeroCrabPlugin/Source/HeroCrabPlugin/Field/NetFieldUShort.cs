using System;
// ReSharper disable once CheckNamespace


public class NetFieldUShort : NetField, INetFieldUShort
{
    internal Action<ushort> Receive { get; set; }

    private readonly NetFieldBuffer<ushort> _buffer;

    public NetFieldUShort(byte index, string name, bool isReliable, Action<ushort> callback)
    {
        IsReliable = isReliable;
        Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.UShort);

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<ushort>(bufferSize);
    }

    public NetFieldUShort(NetFieldDesc description, Action<ushort> callback)
    {
        IsReliable = description.IsReliable;
        Description = description;

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<ushort>(bufferSize);
    }

    public void Set(ushort value)
    {
        _buffer.Add(value);
        TxQueue.WriteUShort(value);
        IsUpdated = true;

        LastQueue.Clear();
        LastQueue.WriteUShort(value);
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
            _buffer.Add(rxQueue.ReadUShort());
        }
    }
}
