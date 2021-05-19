using System;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Session
{
    public abstract class NetSession : NetObject, INetSession
    {
        public uint Id { get; private set; }
        public string Ip => _sublayer.Ip;
        public uint RxCount { get; protected set; }
        public uint TxCount { get; private set; }
        public uint MissCount { get; protected set; }
        public uint DupeCount { get; protected set; }
        public Action<INetElement> ElementCreated { get; set; }
        public Action<INetElement> ElementDeleted{ get; set; }
        public Action<NetSession> SessionCreated { get; set; }
        public Action<INetSublayer> SessionDeleted { get; set; }
        public NetStreamGroup StreamGroup { get; set; }

        public enum InnerTypeCode : byte
        {
            Delete,
            Create,
            Modify,
            Input,
        }

        protected readonly NetByteQueue TxQueue;
        protected readonly NetByteQueue RxQueue;
        protected bool IsPacketReliable;

        private readonly INetSublayer _sublayer;

        public virtual void Disconnect()
        {
            _sublayer.Disconnect();
            SessionDeleted?.Invoke(_sublayer);
        }

        public abstract void Send();

        protected void OnReceiveId(uint id)
        {
            Id = id;
            SessionCreated?.Invoke(this);
        }

        protected NetSession(INetSublayer sublayer)
        {
            _sublayer = sublayer;
            _sublayer.ReceiveDataCallback = OnReceivePacket;

            Id = _sublayer.Id;

            TxQueue = new NetByteQueue();
            RxQueue = new NetByteQueue();

            StreamGroup = NetStreamGroup.Default;
        }

        protected virtual void SendPacket(byte[] packet, bool isReliable)
        {
            TxCount++;
            _sublayer.Send(packet, isReliable);
        }

        protected abstract void OnReceivePacket(byte[] packet);
    }
}
