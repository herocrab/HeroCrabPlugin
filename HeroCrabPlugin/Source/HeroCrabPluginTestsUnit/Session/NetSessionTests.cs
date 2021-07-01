using System.Collections.Generic;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;
using Moq;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Session
{
    [TestFixture]
    public class NetSessionTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Disconnect_CalledMethod_VerifySublayerMethodCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 0;

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            sessionA.Disconnect();
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Once);
        }

        [Test]
        public void Constructor_SublayerIsProvided_VerifyIpIsThatOfSublayer()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 0;
            fakeSublayerA.Setup(x => x.Ip).Returns("127.0.0.1");

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            Assert.That(sessionA.Ip, Is.EqualTo("127.0.0.1"));
        }
    }
}
