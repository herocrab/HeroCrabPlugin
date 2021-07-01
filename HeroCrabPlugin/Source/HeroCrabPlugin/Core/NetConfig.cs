/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
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
        public ushort RegisterPort { get; set; }

        /// <summary>
        /// Server listener port.
        /// </summary>
        public ushort CatalogPort { get; set; }

        /// <summary>
        /// Server listener port.
        /// </summary>
        public ushort ServerPort { get; set; }

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
            ushort registerPort = 42056,
            ushort catalogPort = 42057,
            ushort serverPort = 42058,
            ushort connections = 100,
            ushort log = 1000)
        {
            Role = role;
            RegisterPort = registerPort;
            CatalogPort = catalogPort;
            ServerPort = serverPort;
            Connections = connections;
            Log = log;
            Address = address;
            Name = name;
            Map = map;
        }
    }
}
