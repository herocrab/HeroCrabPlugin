using System;

namespace HeroCrabPlugin.Core
{
    public static class NetBootStrap
    {
        public static NetBootConfig Config { get; private set; }

        public static bool Initialize(string filename)
        {
            try {
                Config = NetBootConfig.Read(filename);
                return true;
            }
            catch (Exception) {
                Config = new NetBootConfig();
                return false;
            }
        }
    }
}
