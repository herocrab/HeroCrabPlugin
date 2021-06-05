using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;

namespace HeroCrabPlugin.Stream
{
    /// <inheritdoc />
    public abstract class NetStream : NetObject
    {
        /// <inheritdoc />
        public delegate void SessionConnectedHandler(INetSession session);

        /// <inheritdoc />
        public delegate void SessionDisconnectedHandler(INetSession session);

        /// <inheritdoc />
        public delegate void ElementCreatedHandler(INetElement element);

        /// <inheritdoc />
        public delegate void ElementDeletedHandler(INetElement element);

        /// <summary>
        /// Event is invoked when a session is connected.
        /// </summary>
        public event SessionConnectedHandler SessionConnected;

        /// <summary>
        /// Event is invoked when a session is disconnected.
        /// </summary>
        public event SessionDisconnectedHandler SessionDisconnected;

        /// <summary>
        /// Count of all network elements in the stream.
        /// </summary>
        public int ElementCount => Elements.Count;

        /// <summary>
        /// Count of all network sessions in the stream.
        /// </summary>
        public int SessionCount => Sessions.Count;

        /// <summary>
        /// Interval at which packets are sent as it relates to game tick.
        /// </summary>
        protected int PacketInterval { get; set; }

        /// <summary>
        /// Sessions added to this stream.
        /// </summary>
        protected readonly SortedDictionary<uint, NetSession> Sessions;

        /// <summary>
        /// Elements added to this stream.
        /// </summary>
        protected readonly SortedDictionary<uint, NetElement> Elements;

        private ulong _tick;

        /// <inheritdoc />
        protected NetStream()
        {
            Sessions = new SortedDictionary<uint, NetSession>();
            Elements = new SortedDictionary<uint, NetElement>();
        }

        /// <summary>
        /// Kick all sessions from the stream.
        /// </summary>
        public void KickAll()
        {
            foreach (var networkSession in Sessions.Values.ToArray()) {
                networkSession.Disconnect();
            }
        }

        /// <summary>
        /// Add a session to this stream.
        /// </summary>
        /// <param name="session">Network session</param>
        protected virtual void AddSession(NetSession session)
        {
            Sessions.Add(session.Id, session);
            SessionConnected?.Invoke(session);

            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Session ({session.Id}) has been added to the stream, there are now {Sessions.Count} total sessions.");
        }

        /// <summary>
        /// Delete a session from this stream.
        /// </summary>
        /// <param name="sublayer">Network sublayer</param>
        public virtual void DeleteSession(INetSublayer sublayer)
        {
            if (!Sessions.ContainsKey(sublayer.Id)) {
                return;
            }

            var session = Sessions[sublayer.Id];
            Sessions.Remove(sublayer.Id);

            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Session ({session.Id}) has been removed from the stream, there are now {Sessions.Count} total sessions.");

            SessionDisconnected?.Invoke(session);
            DeleteAuthoredElements(session);
        }

        /// <summary>
        /// Process the network stream.
        /// </summary>
        /// <param name="time"></param>
        public virtual void Process(float time)
        {
            ProcessElements();
            SendPacketSubRate();
            _tick++;
        }

        /// <summary>
        /// Clear all elements from the network stream.
        /// </summary>
        public void Clear()
        {
            Elements.Clear();
        }

        /// <summary>
        /// Send elements to sessions, subject to filtering based on stream group.
        /// </summary>
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
}
