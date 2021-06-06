using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Server registration message; sent from game server to register server.
    /// </summary>
    public class ServerRegistration
    {
        /// <summary>
        /// Network element associated with this class; used for removal.
        /// </summary>
        public INetElement Element { get; }

        /// <summary>
        /// Server registration message; sent from game server to register server.
        /// </summary>
        /// <param name="registerServer">This registration server</param>
        /// <param name="session">Originator of registration</param>
        /// <param name="advertisement">Corresponding advertisement</param>
        public ServerRegistration(INetServer registerServer, INetSession session, ServerAdvertisement advertisement)
        {
            // Create the registration message and set the author id
            Element = registerServer.Stream.CreateElement("Registration", 0, session.Id, true);
            Element.AddString("Version", true, advertisement.SetVersion);
            Element.AddString("Name", true, advertisement.SetName);
            Element.AddString("Address", true, advertisement.SetAddress);
            Element.AddUShort("Port", true, advertisement.SetPort);
            Element.AddString("Map", true, advertisement.SetMap);
            Element.AddUShort("CurrentPlayers", true, advertisement.SetCurrentPlayers);
            Element.AddUShort("MaxPlayers", true, advertisement.SetMaxPlayers);

            Element.Filter.Recipient = session.Id; // Send to only the author - "Single recipient"
            Element.Filter.StreamGroup = NetStreamGroup.Lobby;
        }
    }
}
