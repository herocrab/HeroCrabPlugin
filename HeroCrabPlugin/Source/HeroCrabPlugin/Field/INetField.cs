// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace HeroCrabPlugin.Field
{
    /// <summary>
    /// Network field.
    /// </summary>
    public interface INetField<in T>
    {
        /// <summary>
        /// Set the network field.
        /// </summary>
        /// <param name="value">Value</param>
        void Set(T value);
    }
}
