// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;
using HeroCrabPlugin.Sublayer.Replay;

// ReSharper disable SuggestBaseTypeForParameter

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

        /// <inheritdoc />
        public INetRecorder Recorder => _recorder;

        private readonly NetRecorder _recorder;
        private readonly SortedDictionary<uint, List<NetElement>> _send;
        private readonly SortedDictionary<uint, List<NetElement>> _exclude;

        private uint _elementIndex;
        private uint _sessionId;

        /// <inheritdoc />
        public NetStreamServer()
        {
            PacketInterval = (int)NetSettings.GameTickRate / (int)NetSettings.ClientPps;

            _send = new SortedDictionary<uint, List<NetElement>> {{0, new List<NetElement>()}};
            _exclude = new SortedDictionary<uint, List<NetElement>> {{0, new List<NetElement>()}};

            //_recorder = new NetRecorder();
            //CreateSession(_recorder);
        }

        /// <inheritdoc />
        protected override void SendElements(float time)
        {
            // This method prepares elements for sending using an optimized dictionary approach to reduce iteration

            // Candidate Matrix:
            // Server(0) to All Recipient(0) with no Exclude = Queue "0"
            // Author(X) to All Recipient(0) with Exclude = Individual queues minus excluded queue
            // Any Author to Recipient(Y) = Individual queue

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

            // Send the elements, the _send and _exclude lists are injected into session(s) at constructor
            foreach (var session in Sessions.Values) {
                session.Send(time);
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
        /// <param name="sublayer"></param>
        public override void DeleteSession(INetSublayer sublayer)
        {
            _send.Remove(sublayer.Id);
            _exclude.Remove(sublayer.Id);

            DeleteAuthoredElements(sublayer.Id);
            base.DeleteSession(sublayer);
        }

        /// <summary>
        /// Create a session on the server given a sublayer.
        /// </summary>
        /// <param name="sublayer"></param>
        /// <returns>Server session</returns>
        public NetSessionServer CreateSession(INetSublayer sublayer)
        {
            var session = new NetSessionServer(sublayer, _send, _exclude)
            {
                ElementCreated = createdElement => ElementCreated?.Invoke(createdElement),
                ElementDeleted = deletedElement => ElementDeleted?.Invoke(deletedElement)
            };
            session.SessionCreated += AddSession;
            session.SessionDeleted += DeleteSession;

            // Skip session "0" for server/network recorder
            if (_sessionId == uint.MaxValue) {
                _sessionId = 1;
            }
            else {
                _sessionId++;
            }

            // Account for possibility of rolling over session id's and still having active sessions
            if (Sessions.ContainsKey(_sessionId)) {
                NetLogger.Write(NetLogger.LoggingGroup.Stream, this,
                    $"Server attempted to assign an existing session id, time for maintenance " +
                    $"or better denial of service protection!");
                sublayer.Disconnect();
                return null;
            }

            sublayer.SendId(_sessionId);
            return session;
        }

        /// <inheritdoc />
        public INetElement CreateElement(string name, uint assetId, uint authorId = 0, bool isEnabled = true,
            INetElement sibling = null)
        {
            var elementDesc = new NetElementDesc(_elementIndex, name, authorId, assetId);
            var element = new NetElement(elementDesc)
            {
                DeleteElement = OnDeleteElement,
                Enabled = isEnabled,
                Sibling = sibling,
                IsServer = true,
                IsClient = false,
            };

            Elements.Add(_elementIndex, element);
            _elementIndex++;

            ElementCreated?.Invoke(element);
            return element;
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

        private void OnDeleteElement(INetElement element)
        {
            if (!Elements.ContainsKey(element.Description.Id)) {
                return;
            }

            Elements.Remove(element.Description.Id);
            ElementDeleted?.Invoke(element);
        }

        private void DeleteAuthoredElements(uint authorId)
        {
            var authoredElements = Elements.Values.Where(element =>
                element.Description.AuthorId == authorId);

            foreach (var element in authoredElements.ToArray()) {
                OnDeleteElement(element);
            }
        }
    }
}
