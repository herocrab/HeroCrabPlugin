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
            NetBoot.ParseCommandLine("role:client");
            Assert.That(NetBoot.Config.Role == "client");
        }
    }
}
