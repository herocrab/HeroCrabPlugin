// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.


using System;

namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network field.
    /// </summary>
    public interface INetField<in T>
    {
        /// <summary>
        /// Set the network field. This will be streamed.
        /// </summary>
        /// <param name="value">Value</param>
        void Set(T value);

        /// <summary>
        /// Set the most recent value, this is not streamed but will be sent to new sessions only.
        /// </summary>
        /// <param name="value">Last Value</param>
        void SetLastValue(T value);
    }

    /// <summary>
    /// Network field receiver callback interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INetFieldReceiver<T>
    {
        /// <summary>
        /// Network field receive call back.
        /// </summary>
        Action<T> Receive { get; set; }
    }
}
