using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetBootStrapTests
    {
        [Test]
        public void Initialize_WithGoodFilePath_VerifyTrueIsReturnedAndConfigIsClient()
        {
            NetBootConfig.Write("client.json");

            //File.WriteAllText("test-file.txt","Test");
            var isAvailable = NetBootStrap.Initialize("client.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "client");
        }

        [Test]
        public void Initialize_WithBadFilePath_VerifyFalseIsReturnedAndConfigIsClient()
        {
            //File.WriteAllText("test-file.txt","Test");
            var isAvailable = NetBootStrap.Initialize("bad-path.json");
            Assert.That(isAvailable, Is.False);
            Assert.That(NetBootStrap.Config.Role == "client");
        }
    }
}
