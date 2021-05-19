using System;
// ReSharper disable once CheckNamespace

public interface INetElement
{
    NetElementDesc Description { get; }
    NetElementFilter Filter { get; }
    int FieldCount { get; }
    bool Enabled { get; set; }
    bool IsReliable { get; }
    bool IsServer { get; }
    bool IsClient { get; }
    INetFieldByte AddByte(string name, bool isReliable, Action<byte> callback);
    INetFieldBytes AddBytes(string name, bool isReliable, Action<byte[]> callback);
    INetFieldFloat AddFloat(string name, bool isReliable, Action<float> callback);
    INetFieldInt AddInt(string name, bool isReliable, Action<int> callback);
    INetFieldLong AddLong(string name, bool isReliable, Action<long> callback);
    INetFieldString AddString(string name, bool isReliable, Action<string> callback);
    INetFieldUInt AddUInt(string name, bool isReliable, Action<uint> callback);
    INetFieldUShort AddUShort(string name, bool isReliable, Action<ushort> callback);
    bool SetActionByte(string name, Action<byte> callback);
    bool SetActionBytes(string name, Action<byte[]> callback);
    bool SetActionFloat(string name, Action<float> callback);
    bool SetActionInt(string name, Action<int> callback);
    bool SetActionLong(string name, Action<long> callback);
    bool SetActionString(string name, Action<string> callback);
    bool SetActionUInt(string name, Action<uint> callback);
    bool SetActionUShort(string name, Action<ushort> callback);
    INetFieldByte GetByte(string name);
    INetFieldBytes GetBytes(string name);
    INetFieldFloat GetFloat(string name);
    INetFieldInt GetInt(string name);
    INetFieldLong GetLong(string name);
    INetFieldString GetString(string name);
    INetFieldUInt GetUInt(string name);
    INetFieldUShort GetUShort(string name);
}
