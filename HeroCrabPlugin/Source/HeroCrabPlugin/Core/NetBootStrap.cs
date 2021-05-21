using System;

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
        public static NetBootConfig Config { get; private set; }

        /// <summary>
        /// Initialize the network configuration.
        /// </summary>
        /// <param name="filePath">Path to the network configuration file</param>
        /// <returns></returns>
        public static bool Initialize(string filePath)
        {
            try {
                NetBootConfig.Read(filePath, out var config);
                Config = config;
                return true;
            }
            catch (Exception) {
                Config = new NetBootConfig();
                return false;
            }
        }
    }
}
