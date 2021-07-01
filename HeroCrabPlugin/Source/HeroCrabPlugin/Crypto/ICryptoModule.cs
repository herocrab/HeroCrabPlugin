/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using System;

namespace HeroCrabPlugin.Crypto
{
    /// <summary>
    /// Crypto module interface.
    /// </summary>
    public interface ICryptoModule
    {
        /// <summary>
        /// Encrypt data bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>Bytes</returns>
        Byte[] Encrypt(byte[] data, string key);

        /// <summary>
        /// Encrypt a string in a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        Byte[] Encrypt(string data, string key);

        /// <summary>
        /// Decrypt bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        Byte[] Decrypt(byte[] data, byte[] key);

        /// <summary>
        /// Decrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>Bytes</returns>
        Byte[] Decrypt(byte[] data, string key);
    }
}
