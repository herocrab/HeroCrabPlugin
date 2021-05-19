using NUnit.Framework;

namespace HeroCrabPlugin.Tests.Unit.Core
{
    [TestFixture]
    public class NetBootStrapTests
    {
        [Test]
        public void Initialize_WithGoodFilePath_VerifyTrueIsReturnedAndConfigIsServer()
        {
            //File.WriteAllText("test-file.txt","Test");
            var isAvailable = NetBootStrap.Initialize("server.json");
            Assert.That(isAvailable, Is.True);
            Assert.That(NetBootStrap.Config.Role == "server");
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
