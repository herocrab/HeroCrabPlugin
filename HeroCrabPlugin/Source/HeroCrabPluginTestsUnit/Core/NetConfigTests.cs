using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetConfigTests
    {
        [Test]
        public void Write_DefaultNetBootConfigAndRead_VerifyFileExists()
        {
            NetConfig.Write("default.json");
            NetConfig.Write("catalog.json");
            NetConfig.Write("server.json");
            NetConfig.Write("client.json");

            var isAvailable = NetBootStrap.Initialize("default.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "client");
        }
    }
}

