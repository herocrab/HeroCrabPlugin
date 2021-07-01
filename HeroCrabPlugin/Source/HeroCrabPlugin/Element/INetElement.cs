// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using System;
using FlaxEngine;
using HeroCrabPlugin.Field;
// ReSharper disable UnusedMemberInSuper.Global

namespace HeroCrabPlugin.Element
{
    /// <summary>
    /// Network element contains network fields (RPC endpoints).
    /// </summary>
    public interface INetElement
    {
        /// <summary>
        /// Network element description, used as ledger in serialization.
        /// </summary>
        NetElementDesc Description { get; }

        /// <summary>
        /// Network element filter, used to filter or mask by sessions.
        /// </summary>
        NetElementFilter Filter { get; }

        /// <summary>
        /// Count of current fields.
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Server only: If true this element will be eligible for streaming.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// If true this element will be sent reliably.
        /// </summary>
        bool IsReliable { get; }

        /// <summary>
        /// IsServer; If true this is the element on the server.
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// IsClient; If true this is the element on the client.
        /// </summary>
        bool IsClient { get; }

        /// <summary>
        /// Server only: Handy sibling element reference; for elements that are related.
        /// </summary>
        INetElement Sibling { get; set; }

        /// <summary>
        /// Delete this element from the server; does nothing on client.
        /// </summary>
        void Delete();

        /// <summary>
        /// Add a byte network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<byte> AddByte(string name, bool isReliable, Action<byte> callback = null);

        /// <summary>
        /// Add a series of bytes as a field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<byte[]> AddBytes(string name, bool isReliable, Action<byte[]> callback = null);

        /// <summary>
        /// Add a float network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<float> AddFloat(string name, bool isReliable, Action<float> callback = null);

        /// <summary>
        /// Add an int network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<int> AddInt(string name, bool isReliable, Action<int> callback = null);

        /// <summary>
        /// Add a long network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<long> AddLong(string name, bool isReliable, Action<long> callback = null);

        /// <summary>
        /// Add a string network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<string> AddString(string name, bool isReliable, Action<string> callback = null);

        /// <summary>
        /// Add a byte network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<uint> AddUInt(string name, bool isReliable, Action<uint> callback = null);

        /// <summary>
        /// Add a ushort network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<ushort> AddUShort(string name, bool isReliable, Action<ushort> callback = null);

        /// <summary>
        /// Add a Vector2 network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<Vector2> AddVector2(string name, bool isReliable, Action<Vector2> callback = null);

        /// <summary>
        /// Add a Vector3 network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<Vector3> AddVector3(string name, bool isReliable, Action<Vector3> callback = null);

        /// <summary>
        /// Add a Vector4 network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<Vector4> AddVector4(string name, bool isReliable, Action<Vector4> callback = null);

        /// <summary>
        /// Add a Quaternion network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<Quaternion> AddQuaternion(string name, bool isReliable, Action<Quaternion> callback = null);

        /// <summary>
        /// Add a bool network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        INetField<bool> AddBool(string name, bool isReliable, Action<bool> callback = null);

        /// <summary>
        /// Set the call back action for a named byte field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionByte(string name, Action<byte> callback);

        /// <summary>
        /// Set the call back action for a named string field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionBytes(string name, Action<byte[]> callback);

        /// <summary>
        /// Set the call back action for a named float field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionFloat(string name, Action<float> callback);

        /// <summary>
        /// Set the call back action for a named int field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionInt(string name, Action<int> callback);

        /// <summary>
        /// Set the call back action for a named long field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionLong(string name, Action<long> callback);

        /// <summary>
        /// Set the call back action for a named string field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionString(string name, Action<string> callback);

        /// <summary>
        /// Set the call back action for a named uint field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionUInt(string name, Action<uint> callback);

        /// <summary>
        /// Set the call back action for a named ushort field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionUShort(string name, Action<ushort> callback);

        /// <summary>
        /// Set the call back action for a named Vector2 field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionVector2(string name, Action<Vector2> callback);

        /// <summary>
        /// Set the call back action for a named Vector3 field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionVector3(string name, Action<Vector3> callback);

        /// <summary>
        /// Set the call back action for a named Vector4 field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionVector4(string name, Action<Vector4> callback);

        /// <summary>
        /// Set the call back action for a named Quaternion field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionQuaternion(string name, Action<Quaternion> callback);

        /// <summary>
        /// Set the call back action for a named bool field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns>Field</returns>
        bool SetActionBool(string name, Action<bool> callback);

        /// <summary>
        /// Retrieves the setter for a byte field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<byte> GetByte(string name);

        /// <summary>
        /// Retrieves the setter for a series of bytes field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<byte[]> GetBytes(string name);

        /// <summary>
        /// Retrieves the setter for a float field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<float> GetFloat(string name);

        /// <summary>
        /// Retrieves the setter for an int field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<int>GetInt(string name);

        /// <summary>
        /// Retrieves the setter for a long field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<long> GetLong(string name);

        /// <summary>
        /// Retrieves the setter for a string field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<string> GetString(string name);

        /// <summary>
        /// Retrieves the setter for a uint field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<uint> GetUInt(string name);

        /// <summary>
        /// Retrieves the setter for a ushort field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<ushort> GetUShort(string name);

        /// <summary>
        /// Retrieves the setter for a Vector2 field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<Vector2> GetVector2(string name);

        /// <summary>
        /// Retrieves the setter for a Vector3 field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<Vector3> GetVector3(string name);

        /// <summary>
        /// Retrieves the setter for a Vector4 field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<Vector4> GetVector4(string name);

        /// <summary>
        /// Retrieves the setter for a Quaternion field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<Quaternion> GetQuaternion(string name);

        /// <summary>
        /// Retrieves the setter for a bool field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Field</returns>
        INetField<bool> GetBool(string name);
    }
}
