// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System.Linq;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;

namespace HeroCrabPlugin.Stream
{
    /// <summary>
    /// Client stream, contains all streamed elements and the client session/
    /// </summary>
    public class NetStreamClient : NetStream, INetStreamClient
    {
        /// <summary>
        /// Event invoked when an element is created; used by game spawner logic.
        /// </summary>
        public event ElementCreatedHandler ElementCreated;

        /// <summary>
        /// Event invoked when an element is deleted; used by game spawner logic.
        /// </summary>
        public event ElementDeletedHandler ElementDeleted;

        /// <summary>
        /// Client stream, contains all streamed elements and the client session; used internally and by unit tests.
        /// </summary>
        public NetStreamClient()
        {
            PacketInterval = (int) NetSettings.GameTickRate / (int) NetSettings.ClientPps;
        }

        /// <summary>
        /// Send all elements to sessions, in this case from the client to the server.
        /// </summary>
        protected override void SendElements(float time)
        {
            foreach (var session in Sessions.Values) {
                session.Send(time);
            }
        }

        /// <summary>
        /// Create a session from a sublayer and add it ot the client.
        /// </summary>
        /// <param name="sublayer"></param>
        /// <returns>Client session</returns>
        public NetSessionClient CreateSession(INetSublayer sublayer)
        {
            var session = new NetSessionClient(sublayer, Elements)
            {
                StreamGroup = NetStreamGroup.Default,
                ElementCreated = createdElement => ElementCreated?.Invoke(createdElement),
                ElementDeleted = deletedElement => ElementDeleted?.Invoke(deletedElement)
            };
            session.SessionCreated += AddSession;
            session.SessionDeleted += DeleteSession;
            return session;
        }

        /// <summary>
        /// Try to find the session on the client; returns null if session is not present.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool FindSession(out INetSession session)
        {
            if (!Sessions.Any()) {
                session = null;
                return false;
            }

            session = Sessions.First().Value;
            return true;
        }
    }
}
