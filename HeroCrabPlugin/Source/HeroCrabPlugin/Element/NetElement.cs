// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Field;
// ReSharper disable NotResolvedInText

namespace HeroCrabPlugin.Element
{
    /// <summary>
    /// Network element contains network fields (RPC endpoints).
    /// </summary>
    public class NetElement : NetObject, INetElement
    {
        /// <inheritdoc />
        public NetElementDesc Description { get; }

        /// <inheritdoc />
        public NetElementFilter Filter { get; }

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public INetElement Sibling { get; set; }

        /// <inheritdoc />
        public bool IsServer { get; internal set; }

        /// <inheritdoc />
        public bool IsClient { get; internal set; }

        /// <summary>
        /// Callback for deleting this element from the server.
        /// </summary>
        public Action<INetElement> DeleteElement { get; internal set; }

        /// <inheritdoc />
        public bool IsReliable => _fields.Values.Any(a => a.IsReliable);

        /// <summary>
        /// IsUpdated is true if this element has any fields which have been updated.
        /// </summary>
        public bool IsUpdated => _fields.Values.Any(a => a.IsUpdated);

        /// <inheritdoc />
        public int FieldCount => _fields.Count;

        private readonly SortedDictionary<byte, NetFieldDesc> _ledger;
        private readonly SortedDictionary<string, NetField> _fields;
        private readonly NetByteQueue _prepQueue;
        private readonly NetByteQueue _txQueue;
        private readonly NetByteQueue _applyQueue;
        private readonly NetByteQueue _fullQueue;

        private byte _fieldIndex;

        /// <inheritdoc />
        public NetElement(NetElementDesc description)
        {
            Description = description;
            Filter = new NetElementFilter();

            _ledger = new SortedDictionary<byte, NetFieldDesc>();
            _fields = new SortedDictionary<string, NetField>();
            _prepQueue = new NetByteQueue();
            _txQueue = new NetByteQueue();
            _applyQueue = new NetByteQueue();
            _fullQueue = new NetByteQueue();

            if (description.HasFields) {
                CreateFields(description);
            }
        }

        /// <inheritdoc />
        public void Delete() => DeleteElement?.Invoke(this);

        /// <inheritdoc />
        public INetFieldByte AddByte(string name, bool isReliable, Action<byte> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldByte(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldBytes AddBytes(string name, bool isReliable, Action<byte[]> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldBytes(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldFloat AddFloat(string name, bool isReliable, Action<float> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldFloat(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldInt AddInt(string name, bool isReliable, Action<int> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldInt(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldLong AddLong(string name, bool isReliable, Action<long> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldLong(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldString AddString(string name, bool isReliable, Action<string> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldString(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldUInt AddUInt(string name, bool isReliable, Action<uint> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldUInt(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldUShort AddUShort(string name, bool isReliable, Action<ushort> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldUShort(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetFieldByte GetByte(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldByte) {
                return _fields[name] as INetFieldByte;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldBytes GetBytes(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldBytes) {
                return _fields[name] as INetFieldBytes;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldFloat GetFloat(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldFloat) {
                return _fields[name] as INetFieldFloat;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldInt GetInt(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldInt) {
                return _fields[name] as INetFieldInt;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldLong GetLong(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldLong) {
                return _fields[name] as INetFieldLong;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldString GetString(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldString) {
                return _fields[name] as INetFieldString;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldUInt GetUInt(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldUInt) {
                return _fields[name] as INetFieldUInt;
            }

            return null;
        }

        /// <inheritdoc />
        public INetFieldUShort GetUShort(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetFieldUShort) {
                return _fields[name] as INetFieldUShort;
            }

            return null;
        }

        /// <inheritdoc />
        public bool SetActionByte(string name, Action<byte> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldByte field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionBytes(string name, Action<byte[]> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldBytes field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionFloat(string name, Action<float> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldFloat field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionInt(string name, Action<int> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldInt field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionLong(string name, Action<long> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldLong field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionString(string name, Action<string> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldString field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionUInt(string name, Action<uint> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldUInt field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionUShort(string name, Action<ushort> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldUShort field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <summary>
        /// Process each of the fields in this element, called by host.
        /// </summary>
        public void Process()
        {
            foreach (var field in _fields.Values.ToArray()) {
                field.Process();
            }
        }

        /// <summary>
        /// Prepare delta changes by serializing only updated fields for this element.
        /// </summary>
        public void PrepareDelta()
        {
            var updatedFields = _fields.Values.Where(a => a.IsUpdated)
                .OrderBy(a => a.Description.Index).ToArray();

            _prepQueue.Clear();
            _prepQueue.WriteInt(updatedFields.Length);

            foreach (var field in updatedFields) {
                _prepQueue.WriteByte(field.Description.Index);
                _prepQueue.WriteRaw(field.Serialize());
            }

            _txQueue.Clear();
            _txQueue.WriteUInt(Description.Id);
            _txQueue.WriteBytes(_prepQueue.ToBytes());
        }

        /// <summary>
        /// Serialize the last field values, used on initial sending of an element.
        /// </summary>
        /// <returns></returns>
        public byte[] SerializeLast()
        {
            _prepQueue.Clear();
            _prepQueue.WriteInt(_fields.Count);

            foreach (var field in _fields.Values) {
                _prepQueue.WriteByte(field.Description.Index);
                _prepQueue.WriteRaw(field.SerializeLast());
            }

            _fullQueue.Clear();
            _fullQueue.WriteUInt(Description.Id);
            _fullQueue.WriteBytes(_prepQueue.ToBytes());
            return _fullQueue.ToBytes();
        }

        /// <summary>
        /// Receive an update and apply it to network fields.
        /// </summary>
        /// <param name="rxQueue">Receive queue</param>
        public void Apply(NetByteQueue rxQueue)
        {
            _applyQueue.Clear();
            _applyQueue.WriteRaw(rxQueue.ReadBytes());

            var count = _applyQueue.ReadInt();

            for (var i = 0; i < count; i++) {
                var index = _applyQueue.ReadByte();

                if (!_ledger.ContainsKey(index)) {
                    continue;
                }

                var name = _ledger[index].Name;
                if (_fields.ContainsKey(name)) {
                    _fields[name].Deserialize(_applyQueue);
                }
            }
        }

        /// <summary>
        /// Serialize the transmit queue.
        /// </summary>
        /// <returns>Bytes</returns>
        public byte[] Serialize() =>_txQueue.ToBytes();

        /// <summary>
        /// Reset all fields IsUpdated values and clear transmit queue.
        /// </summary>
        public void ResetFields()
        {
            foreach (var field in _fields.Values) {
                field.Reset();
            }
        }

        private void ResolveDuplicate(string name)
        {
            if (!_fields.ContainsKey(name)) {
                return;
            }

            _ledger.Remove(_fields[name].Description.Index);
            _fields.Remove(name);
        }

        private void CreateFields(NetElementDesc description)
        {
            _fields.Clear();

            foreach (var field in description.Ledger.Values) {

                switch (field.Type) {

                    case NetFieldDesc.TypeCode.Byte:
                        AddField(new NetFieldByte(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.ByteArray:
                        AddField(new NetFieldBytes(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.Float:
                        AddField(new NetFieldFloat(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.Int:
                        AddField(new NetFieldInt(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.Long:
                        AddField(new NetFieldLong(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.String:
                        AddField(new NetFieldString(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.UInt:
                        AddField(new NetFieldUInt(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.UShort:
                        AddField(new NetFieldUShort(field.Index, field.Name, field.IsReliable, null));
                        break;

                    case NetFieldDesc.TypeCode.Unknown:
                        NetLogger.Write(NetLogger.LoggingGroup.Error,this,"[ERROR] Attempted to create field with unknown type code." );
                        throw new ArgumentOutOfRangeException("[ERROR] Attempted to create field with unknown type code.");
                }
            }
        }

        private void AddField(NetField field)
        {
            _fields.Add(field.Description.Name, field);
            _ledger.Add(field.Description.Index, field.Description);
        }
    }
}
