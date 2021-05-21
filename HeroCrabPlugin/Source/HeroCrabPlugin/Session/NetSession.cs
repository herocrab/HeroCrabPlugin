using System;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Session
{
    /// <summary>
    /// Network session.
    /// </summary>
    public abstract class NetSession : NetObject, INetSession
    {
        /// <inheritdoc />
        public uint Id { get; private set; }

        /// <inheritdoc />
        public string Ip => _sublayer.Ip;

        /// <inheritdoc />
        public uint RxCount { get; protected set; }

        /// <inheritdoc />
        public uint TxCount { get; private set; }

        /// <inheritdoc />
        public uint MissCount { get; protected set; }

        /// <inheritdoc />
        public uint DupeCount { get; protected set; }

        /// <summary>
        /// Call back for element created.
        /// </summary>
        public Action<INetElement> ElementCreated { get; set; }

        /// <summary>
        /// Call back for element deleted.
        /// </summary>
        public Action<INetElement> ElementDeleted{ get; set; }

        /// <summary>
        /// Call back for session created.
        /// </summary>
        public Action<NetSession> SessionCreated { get; set; }

        /// <summary>
        /// Call back for session deleted.
        /// </summary>
        public Action<INetSublayer> SessionDeleted { get; set; }

        /// <inheritdoc />
        public NetStreamGroup StreamGroup { get; set; }

        /// <summary>
        /// Inner type code for routing packets.
        /// </summary>
        public enum InnerTypeCode : byte
        {
            #pragma warning disable 1591
            Delete,
            Create,
            Modify,
            Input
            #pragma warning restore 1591
        }

        /// <summary>
        /// Transmit queue for this session.
        /// </summary>
        protected readonly NetByteQueue TxQueue;

        /// <summary>
        /// Receive queue for this session.
        /// </summary>
        protected readonly NetByteQueue RxQueue;

        /// <summary>
        /// IsPacketReliable is true if this packet should be delivered reliably.
        /// </summary>
        protected bool IsPacketReliable;

        private readonly INetSublayer _sublayer;

        /// <inheritdoc />
        public virtual void Disconnect()
        {
            _sublayer.Disconnect();
            SessionDeleted?.Invoke(_sublayer);
        }

        /// <summary>
        /// Send all elements to this session.
        /// </summary>
        public abstract void Send();

        /// <summary>
        /// Called when a session receives a session id.
        /// </summary>
        /// <param name="id"></param>
        protected void OnReceiveId(uint id)
        {
            Id = id;
            SessionCreated?.Invoke(this);
        }

        /// <inheritdoc />
        protected NetSession(INetSublayer sublayer)
        {
            _sublayer = sublayer;
            _sublayer.ReceiveDataCallback = OnReceivePacket;

            Id = _sublayer.Id;

            TxQueue = new NetByteQueue();
            RxQueue = new NetByteQueue();

            StreamGroup = NetStreamGroup.Default;
        }

        /// <summary>
        /// Send packet to this session through the sublayer.
        /// </summary>
        /// <param name="packet">Packets bytes</param>
        /// <param name="isReliable">Send this packet reliably</param>
        protected virtual void SendPacket(byte[] packet, bool isReliable)
        {
            TxCount++;
            _sublayer.Send(packet, isReliable);
        }

        /// <summary>
        /// Receive a packet into this session.
        /// </summary>
        /// <param name="packet"></param>
        protected abstract void OnReceivePacket(byte[] packet);
    }
}
