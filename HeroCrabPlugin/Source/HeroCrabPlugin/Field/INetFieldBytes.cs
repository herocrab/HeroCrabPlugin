// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network series of bytes field.
    /// </summary>
    public interface INetFieldBytes
    {
        /// <summary>
        /// Set the network series of bytes field.
        /// </summary>
        /// <param name="value">Bytes</param>
        void Set(byte[] value);
    }
}
