// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream;
using HeroCrabPlugin.Sublayer;
using Moq;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Stream
{
    [TestFixture]
    public class NetStreamClientTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Send_SessionInvokeElementCreatedAndDeletedInvokesStreamElementCreated_VerifyCallbacksAreCalled()
        {
            var elementCreatedCounter = 0;
            var elementDeletedCounter = 0;

            void ElementCreated(INetElement element)
            {
                elementCreatedCounter++;
            }

            void ElementDeleted(INetElement element)
            {
                elementDeletedCounter++;
            }

            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var stream = new NetStreamClient();
            stream.ElementCreated += ElementCreated;
            stream.ElementDeleted += ElementDeleted;

            var sessionA = stream.CreateSession(fakeSublayerA.Object);

            sessionA.ElementCreated?.Invoke(new NetElement(new NetElementDesc(0, "Test", 0, 0)));
            sessionA.ElementDeleted?.Invoke(new NetElement(new NetElementDesc(0, "Test", 0, 0)));

            Assert.That(elementCreatedCounter, Is.EqualTo(1));
            Assert.That(elementDeletedCounter, Is.EqualTo(1));
        }
    }
}
