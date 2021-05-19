using NUnit.Framework;

namespace HeroCrabPlugin.Tests.Unit.Core
{
    [TestFixture]
    public class NetConfigurationTests
    {
        [Test]
        public void Constructor_NoArguments_ServerBufferDepthIsThree()
        {
            var networkConfiguration = new NetConfig(NetRole.Server);
            Assert.That(networkConfiguration.ServerBufferDepth, Is.EqualTo(3));
        }

        [Test]
        [TestCase(TickRate.Hz60, HostPps.Hz30, 3)]
        [TestCase(TickRate.Hz60, HostPps.Hz10, 7)]
        [TestCase(TickRate.Hz30, HostPps.Hz30, 2)]
        [TestCase(TickRate.Hz30, HostPps.Hz10, 4)]
        public void Constructor_WithProvidedGameTickRateAndServerPacketRate_ServerBufferDepthIsThree(TickRate gameTickRate,
            HostPps hostPps, byte result)
        {
            var networkConfiguration = new NetConfig(NetRole.Server, gameTickRate, hostPps);
            Assert.That(networkConfiguration.ServerBufferDepth, Is.EqualTo(result));
        }

        [Test]
        [TestCase(TickRate.Hz60, HostPps.Hz30, 3)]
        [TestCase(TickRate.Hz60, HostPps.Hz10, 7)]
        [TestCase(TickRate.Hz30, HostPps.Hz30, 2)]
        [TestCase(TickRate.Hz30, HostPps.Hz10, 4)]
        public void Constructor_WithProvidedGameTickRateAndClientPacketRate_ServerBufferDepthIsThree(TickRate gameTickRate,
            HostPps hostPps, byte result)
        {
            var networkConfiguration = new NetConfig(NetRole.Server, gameTickRate, clientPps: hostPps);
            Assert.That(networkConfiguration.ClientBufferDepth, Is.EqualTo(result));
        }
    }
}
