using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;

namespace HeroCrabPlugin.Element
{
    /// <inheritdoc />
    public class NetElementDesc : NetObject
    {
        /// <summary>
        /// Unique network element id value, assigned by stream.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Network element name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Author id is the id of the session which created this element.
        /// </summary>
        public uint AuthorId { get; }

        /// <summary>
        /// Asset id is a unique id assigned by game logic, which references a prefab or actor.
        /// </summary>
        public uint AssetId { get; }

        /// <summary>
        /// HasFields is true if this element has any network fields added to it.
        /// </summary>
        public bool HasFields => Ledger.Any();

        /// <summary>
        /// Ledger for this element; contains ids for network fields and is used in field creation.
        /// </summary>
        public SortedDictionary<byte, NetFieldDesc> Ledger;

        private readonly NetByteQueue _txQueue;

        /// <inheritdoc />
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

        /// <summary>
        /// Serialize this network element description.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize() => _txQueue.ToBytes();

        /// <summary>
        /// Deserialize this element description from a byte queue.
        /// </summary>
        /// <param name="rxQueue"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Write the ledger for this network element.
        /// </summary>
        /// <param name="ledger"></param>
        public void WriteLedger(SortedDictionary<byte, NetFieldDesc> ledger)
        {
            Ledger = ledger;
            UpdateTxQueue();
        }
    }
}
