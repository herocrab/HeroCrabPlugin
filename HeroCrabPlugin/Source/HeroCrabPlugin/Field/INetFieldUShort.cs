// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network short field.
    /// </summary>
    public interface INetFieldUShort
    {
        /// <summary>
        /// Set the network ushort field.
        /// </summary>
        /// <param name="value">UInt16</param>
        void Set(ushort value);
    }
}
