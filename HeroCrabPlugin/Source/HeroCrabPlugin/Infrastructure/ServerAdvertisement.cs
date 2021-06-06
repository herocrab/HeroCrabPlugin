using System;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Field;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Server advertisement message; advertised to clients.
    /// </summary>
    public class ServerAdvertisement
    {
        /// <summary>
        /// Network element associated with this class; used for removal.
        /// </summary>
        public INetElement Element { get; }

        /// <summary>
        /// Version of the game.
        /// </summary>
        public Action<string> SetVersion => _version.Set;

        /// <summary>
        /// Server instance name.
        /// </summary>
        public Action<string> SetName => _name.Set;

        /// <summary>
        /// Public IPv4 address of the server.
        /// </summary>
        public Action<string> SetAddress => _address.Set;

        /// <summary>
        /// Public UDP or TCP port of the server.
        /// </summary>
        public Action<ushort> SetPort => _port.Set;

        /// <summary>
        /// Game map, level, or world.
        /// </summary>
        public Action<string> SetMap => _map.Set;

        /// <summary>
        /// Current number of players.
        /// </summary>
        public Action<ushort> SetCurrentPlayers => _currentPlayers.Set;

        /// <summary>
        /// Maximum number of supported players.
        /// </summary>
        public Action<ushort> SetMaxPlayers => _maxPlayers.Set;

        private readonly INetFieldString _version;
        private readonly INetFieldString _name;
        private readonly INetFieldString _address;
        private readonly INetFieldString _map;
        private readonly INetFieldUShort _port;
        private readonly INetFieldUShort _currentPlayers;
        private readonly INetFieldUShort _maxPlayers;

        /// <summary>
        /// Server advertisement message; advertised to clients.
        /// </summary>
        /// <param name="catalogServer">This catalog server</param>
        /// <param name="session">Originator of advertisement</param>
        public ServerAdvertisement(INetServer catalogServer, INetSession session)
        {
            session.StreamGroup = NetStreamGroup.Lobby;

            // Create the advertisement element for connected clients
            Element = catalogServer.Stream.CreateElement("Advertisement", 0, 0, true);
            Element.Filter.StreamGroup = NetStreamGroup.Lobby;

            // Cache the setters for update through registration
            _version = Element.AddString("Version", true, null);
            _name = Element.AddString("Name", true, null);
            _address = Element.AddString("Address", true, null);
            _port = Element.AddUShort("Port", true, null);
            _map = Element.AddString("Map", true, null);
            _currentPlayers = Element.AddUShort("CurrentPlayers", true, null);
            _maxPlayers = Element.AddUShort("MaxPlayers", true, null);
        }
    }
}
