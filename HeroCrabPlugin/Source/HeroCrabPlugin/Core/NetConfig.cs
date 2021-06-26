using System.IO;
using FlaxEngine.Json;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network boot configuration; this is the launch .json configuration as an object.
    /// </summary>
    public class NetConfig
    {
        /// <summary>
        /// Role of the host.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Server ip address used as listener for joining a game server.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Server name used to advertise a game server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Server map, this is typically a world, level, terrain, or environment designation.
        /// </summary>
        public string Map { get; set; }

        /// <summary>
        /// Server listener port.
        /// </summary>
        public ushort PortA { get; set; }

        /// <summary>
        /// Server listener port.
        /// </summary>
        public ushort PortB { get; set; }

        /// <summary>
        /// Server listener port.
        /// </summary>
        public ushort PortC { get; set; }

        /// <summary>
        /// Maximum number of connections allowed on this server instance.
        /// </summary>
        public ushort Connections { get; set; }

        /// <summary>
        /// Maximum number of log entries to maintain in the logging buffer.
        /// </summary>
        public ushort Log { get; set; }

        /// <summary>
        /// Network boot configuration; this is the launch .json configuration as an object.
        /// </summary>
        public NetConfig(
            string role = "client",
            string address = "127.0.0.1",
            string name = "HeroCrabPlugin Network Server",
            string map = "DemoMap",
            ushort portA = 42056,
            ushort portB = 42057,
            ushort portC = 42058,
            ushort connections = 100,
            ushort log = 1000)
        {
            Role = role;
            PortA = portA;
            PortB = portB;
            PortC = portC;
            Connections = connections;
            Log = log;
            Address = address;
            Name = name;
            Map = map;
        }
    }
}
