// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

public enum NetRole : byte
{
    Server,
    Client,
}

public enum TickRate : byte
{
    Hz30 = 30,
    Hz60 = 60,
}

public enum HostPps : byte
{
    Hz10 = 10,
    Hz30 = 30,
}

public class NetConfig
{
    public readonly NetRole NetRole;
    public readonly TickRate GameTickRate;
    public readonly HostPps ServerPps;
    public readonly HostPps ClientPps;
    public readonly byte ReliableBufferDepth;
    public readonly byte UnreliableBufferDepth;
    public readonly byte ServerBufferDepth;
    public readonly byte ClientBufferDepth;
    public readonly ushort MaxConnections;

    public NetConfig(NetRole netRole,
        TickRate gameTickRate = TickRate.Hz60,
        HostPps serverPps = HostPps.Hz30,
        HostPps clientPps = HostPps.Hz30,
        byte reliableBufferDepth = byte.MaxValue,
        ushort maxConnections = 160)
    {
        NetRole = netRole;
        GameTickRate = gameTickRate;
        ServerPps = serverPps;
        ClientPps = clientPps;
        ReliableBufferDepth = reliableBufferDepth;
        UnreliableBufferDepth = NetRole == NetRole.Server ? ServerBufferDepth : ClientBufferDepth;
        MaxConnections = maxConnections;

        ServerBufferDepth = (byte)((int) gameTickRate / (int) serverPps + 1);
        ClientBufferDepth = (byte)((int) gameTickRate / (int) clientPps + 1);
    }
}
