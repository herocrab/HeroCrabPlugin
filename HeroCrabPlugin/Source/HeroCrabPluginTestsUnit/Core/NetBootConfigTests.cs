using HeroCrab.Network.Core;
using NUnit.Framework;

namespace HeroCrabPlugin.Tests.Unit.Core
{
    [TestFixture]
    public class NetBootConfigTests
    {
        [Test]
        public void Write_DefaultNetBootConfigAndRead_VerifyFileExists()
        {
            NetBootConfig.Write("default.json");

            var isAvailable = NetBootStrap.Initialize("default.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "client");
        }
    }
}

