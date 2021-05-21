// ReSharper disable SuggestBaseTypeForParameter

using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;

namespace HeroCrabPlugin.Stream
{
    /// <summary>
    /// Network server stream.
    /// </summary>
    public class NetStreamServer : NetStream, INetStreamServer
    {
        /// <inheritdoc />
        public event ElementCreatedHandler ElementCreated;

        /// <inheritdoc />
        public event ElementDeletedHandler ElementDeleted;

        private readonly SortedDictionary<uint, List<NetElement>> _send;
        private readonly SortedDictionary<uint, List<NetElement>> _exclude;

        private uint _elementIndex;
        private uint _sessionId;

        /// <inheritdoc />
        public NetStreamServer()
        {
            PacketInterval = (int)NetConfig.GameTickRate / (int)NetConfig.ClientPps;

            _send = new SortedDictionary<uint, List<NetElement>> {{0, new List<NetElement>()}};
            _exclude = new SortedDictionary<uint, List<NetElement>> {{0, new List<NetElement>()}};
        }

        /// <inheritdoc />
        protected override void SendElements()
        {
            // This method prepares elements for sending using an optimized dictionary approach to reduce iteration

            // Candidate Matrix:
            // Server(0) to All Recipient(0) with no Exclude = Queue "0"
            // Author(X) to All Recipient(0) with Exclude = Individual queues minus excluded queue
            // Any Author to Recipient(Y) - Individual queue

            // Reserved Session Id (0) is the server
            // Recipient(0) is destined for all recipients
            // Exclude(0) includes all recipients, excludes no recipient

            // Clear the send element list for each session
            foreach (var recipientQueue in _send.Values) {
                recipientQueue.Clear();
            }

            // Clear the exclude element list for each session
            foreach (var excludeQueue in _exclude.Values) {
                excludeQueue.Clear();
            }

            // Distribute only enabled elements into egress queues to optimize session filter iteration
            foreach (var element in Elements.Values.Where(element => element.Enabled)) {
                // Process "server to all" elements
                if (element.Filter.Recipient == 0) {
                    _send[0].Add(element);

                    if (element.Filter.Exclude == 0) {
                        continue;
                    }

                    // Update the exclude elements queue
                    if (_exclude.ContainsKey(element.Filter.Exclude)) {
                            _exclude[element.Filter.Exclude].Add(element);
                    }
                }

                // Assign "single recipient" elements to specific queue
                else if (element.Filter.Recipient != 0 && element.Filter.Recipient != element.Filter.Exclude) {
                    if (_send.ContainsKey(element.Filter.Recipient)) {
                        _send[element.Filter.Recipient].Add(element);
                    }
                }
            }

            // Send the elements, the send and exclude lists are injected at constructor
            foreach (var session in Sessions.Values) {
                session.Send();
            }
        }

        /// <inheritdoc />
        protected override void AddSession(NetSession session)
        {
            _send.Add(session.Id, new List<NetElement>());
            _exclude.Add(session.Id, new List<NetElement>());
            base.AddSession(session);
        }

        /// <summary>
        /// Delete a session from this stream given a sublayer.
        /// </summary>
        /// <param name="netSublayer"></param>
        public override void DeleteSession(INetSublayer netSublayer)
        {
            _send.Remove(netSublayer.Id);
            _exclude.Remove(netSublayer.Id);
            base.DeleteSession(netSublayer);
        }

        /// <inheritdoc />
        public NetSessionServer CreateSession(INetSublayer netSublayer)
        {
            var session = new NetSessionServer(netSublayer, _send, _exclude)
            {
                ElementCreated = createdElement => ElementCreated?.Invoke(createdElement),
                ElementDeleted = deletedElement => ElementDeleted?.Invoke(deletedElement)
            };
            session.SessionCreated += AddSession;
            session.SessionDeleted += DeleteSession;

            if (_sessionId == uint.MaxValue) {
                _sessionId = 1;
            }
            else {
                _sessionId++;
            }

            netSublayer.SendId(_sessionId);
            return session;
        }

        /// <inheritdoc />
        public INetElement CreateElement(string name, uint assetId, uint authorId = 0, bool isEnabled = true)
        {
            var elementDesc = new NetElementDesc(_elementIndex, name, authorId, assetId);
            var element = new NetElement(elementDesc)
            {
                Enabled = isEnabled,
                IsServer = true,
                IsClient = false
            };

            Elements.Add(_elementIndex, element);
            _elementIndex++;

            ElementCreated?.Invoke(element);
            return element;
        }

        /// <inheritdoc />
        public void DeleteElement(INetElement element)
        {
            if (!Elements.ContainsKey(element.Description.Id)) {
                return;
            }

            Elements.Remove(element.Description.Id);
            ElementDeleted?.Invoke(element);
        }

        /// <summary>
        /// Set the maximum session id; used in unit testing.
        /// </summary>
        public void SetMaxSessionId()
        {
            // This is here for unit test, testing roll over of session Ids
            _sessionId = uint.MaxValue;
        }

        /// <inheritdoc />
        public bool FindSession(uint id, out INetSession session)
        {
            if (!Sessions.ContainsKey(id)) {
                session = null;
                return false;
            }

            session = Sessions[id];
            return true;
        }
    }
}
