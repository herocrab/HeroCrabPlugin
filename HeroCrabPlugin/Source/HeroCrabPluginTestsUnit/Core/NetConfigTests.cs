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
            var catalog = new NetConfig(role: "catalog");
            NetConfig.Write("catalog.json", catalog);

            var server = new NetConfig(role: "server");
            NetConfig.Write("server.json", server);

            var client = new NetConfig(role: "client");
            NetConfig.Write("client.json", client);

            var isAvailable = NetBootStrap.Initialize("catalog.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "catalog");
        }
    }
}

