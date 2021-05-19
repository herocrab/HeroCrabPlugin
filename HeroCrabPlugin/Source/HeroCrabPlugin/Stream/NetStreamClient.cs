using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;

namespace HeroCrab.Network.Stream
{
    public class NetStreamClient : NetStream, INetStreamClient
    {
        public event ElementCreatedHandler ElementCreated;
        public event ElementDeletedHandler ElementDeleted;

        public NetStreamClient()
        {
            PacketInterval = (int) NetConfig.GameTickRate / (int) NetConfig.ClientPps;
        }

        protected override void SendElements()
        {
            foreach (var session in Sessions.Values) {
                session.Send();
            }
        }

        public NetSessionClient CreateSession(INetSublayer netSublayer)
        {
            var session = new NetSessionClient(netSublayer, Elements)
            {
                ElementCreated = createdElement => ElementCreated?.Invoke(createdElement),
                ElementDeleted = deletedElement => ElementDeleted?.Invoke(deletedElement)
            };
            session.SessionCreated += AddSession;
            session.SessionDeleted += DeleteSession;
            return session;
        }

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
