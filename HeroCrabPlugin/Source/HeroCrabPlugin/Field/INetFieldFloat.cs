// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network float field.
    /// </summary>
    public interface INetFieldFloat
    {
        /// <summary>
        /// Set the network float field.
        /// </summary>
        /// <param name="value">Float</param>
        void Set(float value);
    }
}
