using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;

namespace HeroCrabPlugin.Element
{
    public class NetElementDesc : NetObject
    {
        public uint Id { get; }
        public string Name { get; }
        public uint AuthorId { get; }
        public uint AssetId { get; }
        public bool HasFields => Ledger.Any();

        public SortedDictionary<byte, NetFieldDesc> Ledger;

        private readonly NetByteQueue _txQueue;

        public NetElementDesc(uint id, string name, uint authorId, uint assetId)
        {
            Id = id;
            Name = name;
            AuthorId = authorId;
            AssetId = assetId;

            Ledger = new SortedDictionary<byte, NetFieldDesc>();
            _txQueue = new NetByteQueue();

            UpdateTxQueue();
        }

        private void UpdateTxQueue()
        {
            _txQueue.Clear();
            _txQueue.WriteUInt(Id);
            _txQueue.WriteString(Name);
            _txQueue.WriteUInt(AuthorId);
            _txQueue.WriteUInt(AssetId);
            _txQueue.WriteByte((byte)Ledger.Count);

            foreach (var fieldDesc in Ledger.Values) {
                _txQueue.WriteRaw(fieldDesc.Serialize());
            }
        }

        public byte[] Serialize() => _txQueue.ToBytes();

        public static NetElementDesc Deserialize(NetByteQueue rxQueue)
        {
            var index = rxQueue.ReadUInt();
            var name = rxQueue.ReadString();
            var author = rxQueue.ReadUInt();
            var assetId = rxQueue.ReadUInt();

            var elementDesc = new NetElementDesc(index, name, author, assetId);

            var count = rxQueue.ReadByte();
            var ledger = new SortedDictionary<byte, NetFieldDesc>();
            for (var i = 0; i < count; i++) {
                var entry = NetFieldDesc.Deserialize(rxQueue);
                ledger.Add(entry.Index, entry);
            }

            elementDesc.WriteLedger(ledger);
            return elementDesc;
        }

        public void WriteLedger(SortedDictionary<byte, NetFieldDesc> ledger)
        {
            Ledger = ledger;
            UpdateTxQueue();
        }
    }
}
