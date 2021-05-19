using System.Collections.Generic;
using System.Linq;
// ReSharper disable once CheckNamespace

public abstract class NetStream : NetObject
    {
        public delegate void SessionConnectedHandler(INetSession netSession);
        public delegate void SessionDisconnectedHandler(INetSession netSession);

        public delegate void ElementCreatedHandler(INetElement netElement);
        public delegate void ElementDeletedHandler(INetElement netElement);

        public event SessionConnectedHandler SessionConnected;
        public event SessionDisconnectedHandler SessionDisconnected;

        public int ElementCount => Elements.Count;
        public int SessionCount => Sessions.Count;
        protected int PacketInterval { get; set; }

        protected readonly SortedDictionary<uint, NetSession> Sessions;
        protected readonly SortedDictionary<uint, NetElement> Elements;

        private ulong _tick;

        protected NetStream()
        {
            Sessions = new SortedDictionary<uint, NetSession>();
            Elements = new SortedDictionary<uint, NetElement>();
        }

        public void KickAll()
        {
            foreach (var networkSession in Sessions.Values.ToArray()) {
                networkSession.Disconnect();
            }
        }

        protected virtual void AddSession(NetSession session)
        {
            Sessions.Add(session.Id, session);
            SessionConnected?.Invoke(session);

            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Session ({session.Id}) has been added to the stream, there are now {Sessions.Count} total sessions.");
        }

        public virtual void DeleteSession(INetSublayer netSublayer)
        {
            if (!Sessions.ContainsKey(netSublayer.Id)) {
                return;
            }

            var session = Sessions[netSublayer.Id];
            Sessions.Remove(netSublayer.Id);

            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Session ({session.Id}) has been removed from the stream, there are now {Sessions.Count} total sessions.");

            SessionDisconnected?.Invoke(session);
            DeleteAuthoredElements(session);
        }

        public virtual void Process(float time)
        {
            ProcessElements();
            SendPacketSubRate();
            _tick++;
        }

        public void Clear()
        {
            Elements.Clear();
        }

        protected abstract void SendElements();

        private void ProcessElements()
        {
            foreach (var element in Elements.Values.ToArray()) {
                element.Process();
            }
        }

        private void SendPacketSubRate()
        {
            if (_tick % (ulong) PacketInterval != 0) {
                return;
            }

            PrepareDeltas();
            SendElements();
            ResetFields();
        }

        private void ResetFields()
        {
            foreach (var element in Elements) {
                element.Value.ResetFields();
            }
        }

        private void PrepareDeltas()
        {
            foreach (var element in Elements) {
                element.Value.PrepareDelta();
            }
        }

        private void DeleteAuthoredElements(INetSession session)
        {
            var authoredElements = Elements.Values.Where(element =>
                element.Description.AuthorId == session.Id);

            foreach (var element in authoredElements.ToArray()) {
                Elements.Remove(element.Description.Id);
            }
        }
    }
