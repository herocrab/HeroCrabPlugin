using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetBootConfigTests
    {
        [Test]
        public void Write_DefaultNetBootConfigAndRead_VerifyFileExists()
        {
            NetBootConfig.Write("default.json");
            // NetBootConfig.Write("catalog.json");
            // NetBootConfig.Write("server.json");
            // NetBootConfig.Write("client.json");

            var isAvailable = NetBootStrap.Initialize("default.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "client");
        }
    }
}

