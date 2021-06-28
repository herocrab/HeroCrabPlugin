// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using System.Text;

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Boot strap class used to read and store network configuration.
    /// </summary>
    public static class NetBoot
    {
        /// <summary>
        /// Network configuration.
        /// </summary>
        public static NetConfig Config { get; set; }

        /// <summary>
        /// Initialize the network configuration.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns></returns>
        public static string ParseCommandLine(string args)
        {
            Config = new NetConfig();

            // If this grows parse into a dictionary and use key-lookup
            var commands = args.Split(' ');
            var helpFlag = commands.Any(a => a == "-h" || a == "--h" || a == "-help");
            var name = commands.FirstOrDefault(a => a.Contains("name:"))?.Split(':')[1];
            var role = commands.FirstOrDefault(a => a.Contains("role:"))?.Split(':')[1].ToLower();
            var address = commands.FirstOrDefault(a => a.Contains("address:"))?.Split(':')[1].ToLower();
            var registerPort = commands.FirstOrDefault(a => a.Contains("registerPort:"))?.Split(':')[1];
            var catalogPort = commands.FirstOrDefault(a => a.Contains("catalogPort:"))?.Split(':')[1];
            var serverPort = commands.FirstOrDefault(a => a.Contains("serverPort:"))?.Split(':')[1];
            var map = commands.FirstOrDefault(a => a.Contains("map:"))?.Split(':')[1];
            var connections = commands.FirstOrDefault(a => a.Contains("connections:"))?.Split(':')[1];
            var log = commands.FirstOrDefault(a => a.Contains("log:"))?.Split(':')[1];

            if (helpFlag) {
                Config.Role = "help";
                return PrintHelp();
            }

            try {
                if (name != null) {
                    Config.Name = name;
                }

                if (role != null) {
                    Config.Role = role;
                }

                if (address != null) {
                    Config.Address = address;
                }

                if (registerPort != null) {
                    Config.RegisterPort = Convert.ToUInt16(registerPort);
                }

                if (catalogPort != null) {
                    Config.CatalogPort = Convert.ToUInt16(catalogPort);
                }

                if (serverPort != null) {
                    Config.ServerPort = Convert.ToUInt16(serverPort);
                }

                if (map != null) {
                    Config.Map = map;
                }

                if (connections != null) {
                    Config.Connections = Convert.ToUInt16(connections);
                }

                if (log != null) {
                    Config.Log = Convert.ToUInt16(log);
                }

            }
            catch {
                Config.Role = "help";
                return PrintHelp();
            }

            return "Boot initialization complete!";
        }

        private static string PrintHelp()
        {
            var manual = new StringBuilder("\n\nAvailable command line arguments:\n");
            manual.Append("\t \"name:HeroCrab_Server\"\n");
            manual.Append("\t \"role:catalog\"\n");
            manual.Append("\t \"role:server\"\n");
            manual.Append("\t \"role:client\"\n");
            manual.Append("\t \"address:127.0.0.1\"\n");
            manual.Append("\t \"registerPort:42056\"\n");
            manual.Append("\t \"catalogPort:42057\"\n");
            manual.Append("\t \"serverPort:42058\"\n");
            manual.Append("\t \"map:DemoMap\"\n");
            manual.Append("\t \"connections:200\"\n");
            manual.Append("\t \"log:1000\"\n\n");
            return manual.ToString();
        }
    }
}
