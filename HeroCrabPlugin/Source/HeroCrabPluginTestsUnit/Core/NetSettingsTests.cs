/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using HeroCrabPlugin.Core;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Core
{
    [TestFixture]
    public class NetSettingsTests
    {
        [Test]
        public void Constructor_NoArguments_ServerBufferDepthIsThree()
        {
            var netSettings = new NetSettings();
            Assert.That(netSettings.ServerBufferDepth, Is.EqualTo(3));
        }

        [Test]
        [TestCase(TickRate.Hz60, HostPps.Hz30, 3)]
        [TestCase(TickRate.Hz60, HostPps.Hz10, 7)]
        [TestCase(TickRate.Hz30, HostPps.Hz30, 2)]
        [TestCase(TickRate.Hz30, HostPps.Hz10, 4)]
        public void Constructor_WithProvidedGameTickRateAndServerPacketRate_ServerBufferDepthIsThree(
            TickRate gameTickRate,
            HostPps hostPps, byte result)
        {
            var netSettings = new NetSettings(gameTickRate, hostPps);
            Assert.That(netSettings.ServerBufferDepth, Is.EqualTo(result));
        }

        [Test]
        [TestCase(TickRate.Hz60, HostPps.Hz30, 3)]
        [TestCase(TickRate.Hz60, HostPps.Hz10, 7)]
        [TestCase(TickRate.Hz30, HostPps.Hz30, 2)]
        [TestCase(TickRate.Hz30, HostPps.Hz10, 4)]
        public void Constructor_WithProvidedGameTickRateAndClientPacketRate_ServerBufferDepthIsThree(
            TickRate gameTickRate,
            HostPps hostPps, byte result)
        {
            var netSettings = new NetSettings(gameTickRate, clientPps: hostPps);
            Assert.That(netSettings.ClientBufferDepth, Is.EqualTo(result));
        }
    }
}
