// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable TooWideLocalVariableScope
// ReSharper disable SuggestBaseTypeForParameter

namespace HeroCrabPlugin.Crypto
{
    /// <summary>
    /// Crypto module implementing XXTEA PSK cryptography; Developed by David J. Wheeler and Roger M. Needham.
    /// Algorithm code authored by Ma Bingyao (mabingyao@gmail.com).
    /// </summary>
    public class XxteaCryptoModule : ICryptoModule
    {
        private readonly UTF8Encoding _utf8 = new UTF8Encoding();
        private const UInt32 Delta = 0x9E3779B9;

        private static uint Mx(uint sum, uint y, uint z, int p, uint e, uint[] k)
        {
            return (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
        }

        /// <summary>
        /// Encrypt data bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            return data.Length == 0 ? data : ToByteArray(Encrypt(ToUInt32Array(data, true),
                ToUInt32Array(FixKey(key), false)), false);
        }

        /// <summary>
        /// Encrypt a string with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        public byte[] Encrypt(string data, byte[] key)
        {
            return Encrypt(_utf8.GetBytes(data), key);
        }

        /// <summary>
        /// Encrypt data bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>Bytes</returns>
        public byte[] Encrypt(byte[] data, string key)
        {
            return Encrypt(data, _utf8.GetBytes(key));
        }

        /// <summary>
        /// Encrypt a string in a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        public byte[] Encrypt(string data, string key)
        {
            return Encrypt(_utf8.GetBytes(data), _utf8.GetBytes(key));
        }

        /// <summary>
        /// Encrypt bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>String in base-64 format</returns>
        public string EncryptToBase64String(byte[] data, byte[] key)
        {
            return Convert.ToBase64String(Encrypt(data, key));
        }

        /// <summary>
        /// Encrypt a string with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>String in base-64 format</returns>
        public string EncryptToBase64String(string data, byte[] key)
        {
            return Convert.ToBase64String(Encrypt(data, key));
        }

        /// <summary>
        /// Encrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>String in base-64 format</returns>
        public string EncryptToBase64String(byte[] data, string key)
        {
            return Convert.ToBase64String(Encrypt(data, key));
        }

        /// <summary>
        /// Encrypt a string with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>String in base-64 format</returns>
        public string EncryptToBase64String(string data, string key)
        {
            return Convert.ToBase64String(Encrypt(data, key));
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            return data.Length == 0 ? data : ToByteArray(Decrypt(ToUInt32Array(data, false),
                ToUInt32Array(FixKey(key), false)), true);
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>Bytes</returns>
        public byte[] Decrypt(byte[] data, string key)
        {
            return Decrypt(data, _utf8.GetBytes(key));
        }

        /// <summary>
        /// Decrypt string with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>Bytes</returns>
        public byte[] DecryptBase64String(string data, byte[] key)
        {
            return Decrypt(Convert.FromBase64String(data), key);
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>Bytes</returns>
        public byte[] DecryptBase64String(string data, string key)
        {
            return Decrypt(Convert.FromBase64String(data), key);
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>String in base-64 format</returns>
        public string DecryptToString(byte[] data, byte[] key)
        {
            return _utf8.GetString(Decrypt(data, key));
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in bytes format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>String in base-64 format</returns>
        public string DecryptToString(byte[] data, string key)
        {
            return _utf8.GetString(Decrypt(data, key));
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in bytes format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in bytes format</param>
        /// <returns>String in base-64 format</returns>
        public string DecryptBase64StringToString(string data, byte[] key)
        {
            return _utf8.GetString(DecryptBase64String(data, key));
        }

        /// <summary>
        /// Decrypt bytes with a pre-shared key in string format.
        /// </summary>
        /// <param name="data">Data in string format</param>
        /// <param name="key">Key in string format</param>
        /// <returns>String in base-64 format</returns>
        public string DecryptBase64StringToString(string data, string key)
        {
            return _utf8.GetString(DecryptBase64String(data, key));
        }

        private static uint[] Encrypt(uint[] v, uint[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1) {
                return v;
            }
            UInt32 z = v[n], y, sum = 0, e;
            Int32 p, q = 6 + 52 / (n + 1);
            unchecked {
                while (0 < q--) {
                    sum += Delta;
                    e = sum >> 2 & 3;
                    for (p = 0; p < n; p++) {
                        y = v[p + 1];
                        z = v[p] += Mx(sum, y, z, p, e, k);
                    }
                    y = v[0];
                    z = v[n] += Mx(sum, y, z, p, e, k);
                }
            }
            return v;
        }

        private static uint[] Decrypt(uint[] v, uint[] k)
        {
            Int32 n = v.Length - 1;
            if (n < 1) {
                return v;
            }
            UInt32 z, y = v[0], sum, e;
            Int32 p, q = 6 + 52 / (n + 1);
            unchecked {
                sum = (UInt32)(q * Delta);
                while (sum != 0) {
                    e = sum >> 2 & 3;
                    for (p = n; p > 0; p--) {
                        z = v[p - 1];
                        y = v[p] -= Mx(sum, y, z, p, e, k);
                    }
                    z = v[n];
                    y = v[0] -= Mx(sum, y, z, p, e, k);
                    sum -= Delta;
                }
            }
            return v;
        }

        private static byte[] FixKey(byte[] key)
        {
            if (key.Length == 16) return key;
            Byte[] fixedKey = new Byte[16];
            if (key.Length < 16) {
                key.CopyTo(fixedKey, 0);
            }
            else {
                Array.Copy(key, 0, fixedKey, 0, 16);
            }
            return fixedKey;
        }

        private static uint[] ToUInt32Array(IReadOnlyList<byte> data, bool includeLength)
        {
            Int32 length = data.Count;
            Int32 n = (((length & 3) == 0) ? (length >> 2) : ((length >> 2) + 1));
            UInt32[] result;
            if (includeLength) {
                result = new UInt32[n + 1];
                result[n] = (UInt32)length;
            }
            else {
                result = new UInt32[n];
            }
            for (Int32 i = 0; i < length; i++) {
                result[i >> 2] |= (UInt32)data[i] << ((i & 3) << 3);
            }
            return result;
        }

        private static byte[] ToByteArray(IReadOnlyList<uint> data, bool includeLength)
        {
            Int32 n = data.Count << 2;
            if (includeLength) {
                Int32 m = (Int32)data[data.Count - 1];
                n -= 4;
                if ((m < n - 3) || (m > n)) {
                    return null;
                }
                n = m;
            }
            Byte[] result = new Byte[n];
            for (Int32 i = 0; i < n; i++) {
                result[i] = (Byte)(data[i >> 2] >> ((i & 3) << 3));
            }
            return result;
        }
    }
}
