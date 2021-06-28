// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network long field.
    /// </summary>
    public interface INetFieldLong
    {
        /// <summary>
        /// Set the network long field.
        /// </summary>
        /// <param name="value">Int64</param>
        void Set(long value);
    }
}
