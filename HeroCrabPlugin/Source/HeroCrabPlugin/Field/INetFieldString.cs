// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network string field.
    /// </summary>
    public interface INetFieldString
    {
        /// <summary>
        /// Set the network string field.
        /// </summary>
        /// <param name="value">String</param>
        void Set(string value);
    }
}
