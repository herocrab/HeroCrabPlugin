// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network byte field.
    /// </summary>
    public interface INetFieldByte
    {
        /// <summary>
        /// Set the network bye field.
        /// </summary>
        /// <param name="value">Byte</param>
        void Set(byte value);
    }
}
