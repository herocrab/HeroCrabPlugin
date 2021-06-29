﻿// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine;
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
        public INetField<byte> AddByte(string name, bool isReliable, Action<byte> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldByte(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<byte[]> AddBytes(string name, bool isReliable, Action<byte[]> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldBytes(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<float> AddFloat(string name, bool isReliable, Action<float> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldFloat(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<int> AddInt(string name, bool isReliable, Action<int> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldInt(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<long> AddLong(string name, bool isReliable, Action<long> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldLong(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<string> AddString(string name, bool isReliable, Action<string> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldString(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<uint> AddUInt(string name, bool isReliable, Action<uint> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldUInt(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<ushort> AddUShort(string name, bool isReliable, Action<ushort> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldUShort(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<Vector2> AddVector2(string name, bool isReliable, Action<Vector2> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldVector2(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<Vector3> AddVector3(string name, bool isReliable, Action<Vector3> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldVector3(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<Vector4> AddVector4(string name, bool isReliable, Action<Vector4> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldVector4(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<Quaternion> AddQuaternion(string name, bool isReliable, Action<Quaternion> callback = null)
        {
            ResolveDuplicate(name);
            var field = new NetFieldQuaternion(_fieldIndex, name, isReliable, callback);
            AddField(field);
            _fieldIndex++;

            Description.WriteLedger(_ledger);
            return field;
        }

        /// <inheritdoc />
        public INetField<byte> GetByte(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<byte>) {
                return _fields[name] as INetField<byte>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<byte[]> GetBytes(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<byte[]>) {
                return _fields[name] as INetField<byte[]>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<float> GetFloat(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<float>) {
                return _fields[name] as INetField<float>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<int> GetInt(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<int>) {
                return _fields[name] as INetField<int>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<long> GetLong(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<long>) {
                return _fields[name] as INetField<long>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<string> GetString(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<string>) {
                return _fields[name] as INetField<string>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<uint> GetUInt(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<uint>) {
                return _fields[name] as INetField<uint>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<ushort> GetUShort(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<ushort>) {
                return _fields[name] as INetField<ushort>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<Vector2> GetVector2(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<ushort>) {
                return _fields[name] as INetField<Vector2>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<Vector3> GetVector3(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<ushort>) {
                return _fields[name] as INetField<Vector3>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<Vector4> GetVector4(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<ushort>) {
                return _fields[name] as INetField<Vector4>;
            }

            return null;
        }

        /// <inheritdoc />
        public INetField<Quaternion> GetQuaternion(string name)
        {
            if (_fields.ContainsKey(name) && _fields[name] is INetField<ushort>) {
                return _fields[name] as INetField<Quaternion>;
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

        /// <inheritdoc />
        public bool SetActionVector2(string name, Action<Vector2> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldVector2 field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionVector3(string name, Action<Vector3> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldVector3 field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionVector4(string name, Action<Vector4> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldVector4 field)) {
                return false;
            }

            field.Receive = callback;
            return true;
        }

        /// <inheritdoc />
        public bool SetActionQuaternion(string name, Action<Quaternion> callback)
        {
            if (!_fields.ContainsKey(name)) {
                return false;
            }

            if (!(_fields[name] is NetFieldQuaternion field)) {
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
                        AddField(new NetFieldByte(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.ByteArray:
                        AddField(new NetFieldBytes(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Float:
                        AddField(new NetFieldFloat(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Int:
                        AddField(new NetFieldInt(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Long:
                        AddField(new NetFieldLong(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.String:
                        AddField(new NetFieldString(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.UInt:
                        AddField(new NetFieldUInt(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.UShort:
                        AddField(new NetFieldUShort(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Vector2:
                        AddField(new NetFieldVector2(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Vector3:
                        AddField(new NetFieldVector3(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Vector4:
                        AddField(new NetFieldVector4(field.Index, field.Name, field.IsReliable));
                        break;

                    case NetFieldDesc.TypeCode.Quaternion:
                        AddField(new NetFieldQuaternion(field.Index, field.Name, field.IsReliable));
                        break;

                    default:
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
