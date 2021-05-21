// ReSharper disable MemberCanBePrivate.Global
namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Role of the network host.
    /// </summary>
    public enum NetRole : byte
    {
        /// <summary>
        /// Server role.
        /// </summary>
        Server,

        /// <summary>
        /// Client role.
        /// </summary>
        Client,
    }

    /// <summary>
    /// Game logic update rate.
    /// </summary>
    public enum TickRate : byte
    {
        /// <summary>
        /// Game logic update rate is 30 hertz.
        /// </summary>
        Hz30 = 30,

        /// <summary>
        /// Game logic update rate is 60 hertz.
        /// </summary>
        Hz60 = 60,
    }

    /// <summary>
    /// Game packet rate target.
    /// </summary>
    public enum HostPps : byte
    {
        /// <summary>
        /// Target rate of 10 packets per second.
        /// </summary>
        Hz10 = 10,

        /// <summary>
        /// Target rate of 30 packets per second.
        /// </summary>
        Hz30 = 30,
    }

    /// <summary>
    /// Network configuration parameters.
    /// </summary>
    public class NetConfig
    {
        /// <summary>
        /// Role of the network host.
        /// </summary>
        public readonly NetRole NetRole;

        /// <summary>
        /// Game logic update rate.
        /// </summary>
        public readonly TickRate GameTickRate;

        /// <summary>
        /// Server packet rate target.
        /// </summary>
        public readonly HostPps ServerPps;

        /// <summary>
        /// Client packet rate target.
        /// </summary>
        public readonly HostPps ClientPps;

        /// <summary>
        /// Buffer depth for reliable fields.
        /// </summary>
        public readonly byte ReliableBufferDepth;

        /// <summary>
        /// Buffer depth for unreliable fields.
        /// </summary>
        public readonly byte UnreliableBufferDepth;

        /// <summary>
        /// Unreliable buffer depth for the server.
        /// </summary>
        public readonly byte ServerBufferDepth;

        /// <summary>
        /// Unreliable buffer depth for the client.
        /// </summary>
        public readonly byte ClientBufferDepth;

        /// <summary>
        /// Maximum number of supported connections.
        /// </summary>
        public readonly ushort MaxConnections;

        /// <summary>
        /// Network configuration parameters.
        /// </summary>
        /// <param name="netRole">Role of the network host</param>
        /// <param name="gameTickRate">Game logic update rate</param>
        /// <param name="serverPps">Server packet rate target</param>
        /// <param name="clientPps">Client packet rate target</param>
        /// <param name="reliableBufferDepth">Reliable buffer depth</param>
        /// <param name="maxConnections">Maximum number of supported connections</param>
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
}
