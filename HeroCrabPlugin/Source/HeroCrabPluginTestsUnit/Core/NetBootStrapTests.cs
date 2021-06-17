using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetBootStrapTests
    {
        [Test]
        public void Initialize_WithClientRole_VerifyConfigRoleIsClient()
        {
            NetBootStrap.ParseCommandLine("role:client");
            Assert.That(NetBootStrap.Config.Role == "client");
        }
    }
}
