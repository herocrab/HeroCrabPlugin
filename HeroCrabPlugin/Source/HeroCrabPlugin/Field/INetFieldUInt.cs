// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network Uint field.
    /// </summary>
    public interface INetFieldUInt
    {
        /// <summary>
        /// Set the network uint field.
        /// </summary>
        /// <param name="value">UInt32</param>
        void Set(uint value);
    }
}
