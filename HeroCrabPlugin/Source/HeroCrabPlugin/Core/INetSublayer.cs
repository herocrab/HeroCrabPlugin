using System;
// ReSharper disable once CheckNamespace

public interface INetSublayer
{
    uint Id { get; set; }
    string Ip { get; }
    Action<byte[]> ReceiveDataCallback { get; set; }
    Action<uint> ReceiveIdCallback { get; set; }
    void Disconnect();
    void Send(byte[] packet, bool isReliable);
    void SendId(uint id);
}
