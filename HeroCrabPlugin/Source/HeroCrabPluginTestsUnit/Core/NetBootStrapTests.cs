/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
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
