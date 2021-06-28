// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
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
        /// <returns></returns>
        INetFieldByte AddByte(string name, bool isReliable, Action<byte> callback);

        /// <summary>
        /// Add a series of bytes as a field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldBytes AddBytes(string name, bool isReliable, Action<byte[]> callback);

        /// <summary>
        /// Add a float network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldFloat AddFloat(string name, bool isReliable, Action<float> callback);

        /// <summary>
        /// Add an int network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldInt AddInt(string name, bool isReliable, Action<int> callback);

        /// <summary>
        /// Add a long network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldLong AddLong(string name, bool isReliable, Action<long> callback);

        /// <summary>
        /// Add a string network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldString AddString(string name, bool isReliable, Action<string> callback);

        /// <summary>
        /// Add a byte network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldUInt AddUInt(string name, bool isReliable, Action<uint> callback);

        /// <summary>
        /// Add a ushort network field to this element.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="isReliable">If true this field is sent reliably</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        INetFieldUShort AddUShort(string name, bool isReliable, Action<ushort> callback);

        /// <summary>
        /// Set the call back action for a named byte field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionByte(string name, Action<byte> callback);

        /// <summary>
        /// Set the call back action for a named string field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionBytes(string name, Action<byte[]> callback);

        /// <summary>
        /// Set the call back action for a named float field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionFloat(string name, Action<float> callback);

        /// <summary>
        /// Set the call back action for a named int field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionInt(string name, Action<int> callback);

        /// <summary>
        /// Set the call back action for a named long field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionLong(string name, Action<long> callback);

        /// <summary>
        /// Set the call back action for a named string field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionString(string name, Action<string> callback);

        /// <summary>
        /// Set the call back action for a named uint field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionUInt(string name, Action<uint> callback);

        /// <summary>
        /// Set the call back action for a named ushort field, typically used on the client.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="callback">Call back method invoked when receiving this field</param>
        /// <returns></returns>
        bool SetActionUShort(string name, Action<ushort> callback);

        /// <summary>
        /// Retrieves the setter for a byte field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldByte GetByte(string name);

        /// <summary>
        /// Retrieves the setter for a series of bytes field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldBytes GetBytes(string name);

        /// <summary>
        /// Retrieves the setter for a float field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldFloat GetFloat(string name);

        /// <summary>
        /// Retrieves the setter for an int field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldInt GetInt(string name);

        /// <summary>
        /// Retrieves the setter for a long field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldLong GetLong(string name);

        /// <summary>
        /// Retrieves the setter for a string field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldString GetString(string name);

        /// <summary>
        /// Retrieves the setter for a uint field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldUInt GetUInt(string name);

        /// <summary>
        /// Retrieves the setter for a ushort field.
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns></returns>
        INetFieldUShort GetUShort(string name);
    }
}
