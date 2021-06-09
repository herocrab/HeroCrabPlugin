﻿using System;

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Boot strap class used to read and store network configuration.
    /// </summary>
    public static class NetBootStrap
    {
        /// <summary>
        /// Network configuration.
        /// </summary>
        public static NetConfig Config { get; set; }

        /// <summary>
        /// Initialize the network configuration.
        /// </summary>
        /// <param name="filePath">Path to the network configuration file</param>
        /// <returns></returns>
        public static bool Initialize(string filePath)
        {
            Config = new NetConfig();

            try {
                var isAvailable = NetConfig.Read(filePath, out var config);
                if (isAvailable) {
                    Config = config;
                }
                return isAvailable;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}
