using System.IO;
using FlaxEngine.Json;

// ReSharper disable MemberCanBePrivate.Global

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network boot configuration; this is the launch .json configuration as an object.
    /// </summary>
    public class NetConfig
    {
        /// <summary>
        /// Version of the game.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Role of the host.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Registration server ip address used as listener for registering a game server.
        /// </summary>
        public string RegisterAddress { get; set; }

        /// <summary>
        /// Catalog server ip address used as listener for retrieving a list of game servers.
        /// </summary>
        public string CatalogAddress { get; set; }

        /// <summary>
        /// Server ip address used as listener for joining a game server.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Server name used to advertise a game server.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Server map, this is typically a world, level, terrain, or environment designation.
        /// </summary>
        public string ServerMap { get; set; }

        /// <summary>
        /// Registration server port used as listener for registering a game server.
        /// </summary>
        public ushort RegisterPort { get; set; }

        /// <summary>
        /// Catalog server port used as listener for retrieving a list of game servers.
        /// </summary>
        public ushort CatalogPort { get; set; }

        /// <summary>
        /// Server port used as listener for joining a game.
        /// </summary>
        public ushort ServerPort { get; set; }

        /// <summary>
        /// Maximum number of connections allowed on this server instance.
        /// </summary>
        public ushort MaxConnections { get; set; }

        /// <summary>
        /// Maximum number of catalog entries allowed on this server instance.
        /// </summary>
        public ushort MaxCatalogSize { get; set; }

        /// <summary>
        /// Maximum number of log entries to maintain in the logging buffer.
        /// </summary>
        public ushort MaxLogSize { get; set; }

        /// <summary>
        /// Network boot configuration; this is the launch .json configuration as an object.
        /// </summary>
        public NetConfig(
            string version = "0.01",
            string role = "client",
            string registerAddress = "127.0.0.1",
            string catalogAddress = "127.0.0.1",
            string serverAddress = "127.0.0.1",
            string serverName = "HeroCrabPlugin Network GameServer",
            string serverMap = "DemoMap",
            ushort registerPort = 42056,
            ushort catalogPort = 42057,
            ushort serverPort = 42058,
            ushort maxConnections = 100,
            ushort maxCatalogSize = 100,
            ushort maxLogSize = 1000)
        {
            Version = version;
            Role = role;
            RegisterPort = registerPort;
            CatalogPort = catalogPort;
            ServerPort = serverPort;
            MaxConnections = maxConnections;
            MaxCatalogSize = maxCatalogSize;
            MaxLogSize = maxLogSize;
            RegisterAddress = registerAddress;
            CatalogAddress = catalogAddress;
            ServerAddress = serverAddress;
            ServerName = serverName;
            ServerMap = serverMap;
        }

        /// <summary>
        /// Write a default boot configuration.
        /// </summary>
        /// <param name="filename">Destination file name</param>
        public static bool Write(string filename)
        {
            try
            {
                var config = new NetConfig();
                var jsonString = JsonSerializer.Serialize(config, true);
                File.WriteAllText(filename, jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads a boot configuration from disk.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="config">Boot configuration retrieved from disk</param>
        /// <returns></returns>
        public static bool Read(string filePath, out NetConfig config)
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                config = JsonSerializer.Deserialize<NetConfig>(jsonString);
                return true;
            }
            catch
            {
                config = null;
                return false;
            }
        }
    }
}
