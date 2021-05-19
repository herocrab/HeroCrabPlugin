using System;
// ReSharper disable once CheckNamespace

public class NetFieldFloat: NetField, INetFieldFloat
{
    internal Action<float> Receive { get; set; }

    private readonly NetFieldBuffer<float> _buffer;

    public NetFieldFloat(byte index, string name, bool isReliable, Action<float> callback)
    {
        IsReliable = isReliable;
        Description = new NetFieldDesc(index, name, isReliable, NetFieldDesc.TypeCode.Float);

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<float>(bufferSize);
    }

    public NetFieldFloat(NetFieldDesc description, Action<float> callback)
    {
        IsReliable = description.IsReliable;
        Description = description;

        Receive = callback;
        var bufferSize = IsReliable ? NetConfig.ReliableBufferDepth : NetConfig.UnreliableBufferDepth;
        _buffer = new NetFieldBuffer<float>(bufferSize);
    }

    public void Set(float value)
    {
        _buffer.Add(value);
        TxQueue.WriteFloat(value);
        IsUpdated = true;

        LastQueue.Clear();
        LastQueue.WriteFloat(value);
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
            _buffer.Add(rxQueue.ReadFloat());
        }
    }
}
