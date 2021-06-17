using System;
using System.Linq;
using System.Text;
using FlaxEditor;

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
        /// <param name="args">Command line arguments</param>
        /// <returns></returns>
        public static string ParseCommandLine(string args)
        {
            Config = new NetConfig();

            // If this grows parse into a dictionary and use key-lookup
            var commands = args.Split(' ');
            var helpFlag = commands.Any(a => a == "-h" || a == "--h" || a == "-help");
            var name = commands.FirstOrDefault(a => a.Contains("name:"))?.Split(':')[1].ToLower();
            var role = commands.FirstOrDefault(a => a.Contains("role:"))?.Split(':')[1].ToLower();
            var address = commands.FirstOrDefault(a => a.Contains("address:"))?.Split(':')[1].ToLower();
            var portA = commands.FirstOrDefault(a => a.Contains("port-a:"))?.Split(':')[1].ToLower();
            var portB = commands.FirstOrDefault(a => a.Contains("port-b:"))?.Split(':')[1].ToLower();
            var portC = commands.FirstOrDefault(a => a.Contains("port-c:"))?.Split(':')[1].ToLower();
            var map = commands.FirstOrDefault(a => a.Contains("map:"))?.Split(':')[1].ToLower();
            var connections = commands.FirstOrDefault(a => a.Contains("connections:"))?.Split(':')[1].ToLower();
            var log = commands.FirstOrDefault(a => a.Contains("log:"))?.Split(':')[1].ToLower();

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

                if (portA != null) {
                    Config.PortA = Convert.ToUInt16(portA);
                }

                if (portB != null) {
                    Config.PortB = Convert.ToUInt16(portB);
                }

                if (portC != null) {
                    Config.PortC = Convert.ToUInt16(portC);
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

            return "Boot strap initialization complete!";
        }

        private static string PrintHelp()
        {
            var manual = new StringBuilder("\n\nAvailable command line arguments:\n");
            manual.Append("\t \"name:HeroCrab_Server\"\n");
            manual.Append("\t \"role:catalog\"\n");
            manual.Append("\t \"role:server\"\n");
            manual.Append("\t \"role:client\"\n");
            manual.Append("\t \"address:127.0.0.1\"\n");
            manual.Append("\t \"port-a:42056\"\n");
            manual.Append("\t \"port-b:42057\"\n");
            manual.Append("\t \"port-c:42058\"\n");
            manual.Append("\t \"map:DemoMap\"\n");
            manual.Append("\t \"connections:200\"\n");
            manual.Append("\t \"log:1000\"\n\n");
            return manual.ToString();
        }
    }
}
