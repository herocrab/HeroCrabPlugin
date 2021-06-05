using System;
using System.Collections.Generic;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using Moq;
using NUnit.Framework; // ReSharper disable UnusedVariable

namespace HeroCrabPluginTestsUnit.Session
{
    [TestFixture]
    public class NetSessionClientTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings(NetRole.Server));
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void ReceivePacket_ReceiveTwoPacketThroughSublayerEmulatingClient_VerifyRxCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Delete);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Create);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Modify);
            txQueue.WriteInt(0); // Count

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());

            Assert.That(sessionA.RxCount, Is.EqualTo(2));
        }

        [Test]
        public void Send_SendAPacketWithNoDataOnlyHeaders_NothingIsSent()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            sessionA.Send();

            fakeSublayerA.Verify(a => a.Send(
                It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Exactly(0));

            Assert.That(sessionA.TxCount, Is.EqualTo(0));
        }

        [Test]
        public void Send_ReceiveZeroBytesInSession_AssertThatErrorIsThrown()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes()));
        }

        [Test]
        public void ReceivePacket_ReceiveInvalidInnerTypeCodeForDelete_VerifyErrorIsThrown()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte(9); // Invalid Type Code
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Create);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Modify);
            txQueue.WriteInt(0); // Count

            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes()));
        }

        [Test]
        public void ReceivePacket_ReceiveInvalidInnerTypeCodeForCreate_VerifyErrorIsThrown()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Delete);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte(9); // Invalid Type Code
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Modify);
            txQueue.WriteInt(0); // Count

            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes()));
        }

        [Test]
        public void ReceivePacket_ReceiveInvalidInnerTypeCodeForModify_VerifyErrorIsThrown()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Delete);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Create);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte(9); // Invalid Type Code
            txQueue.WriteInt(0); // Count

            Assert.Throws<ArgumentOutOfRangeException>(() => fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes()));
        }

        [Test]
        public void ReceivePacket_AttemptToModifyAnElementThatDoesNotExist_VerifyReadOccurs()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var elements = new SortedDictionary<uint, NetElement>();
            var sessionA = new NetSessionClient(fakeSublayerA.Object, elements);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Delete);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Create);
            txQueue.WriteInt(0); // Count
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Modify);
            txQueue.WriteInt(1); // Count
            txQueue.WriteBytes(new byte[10]);

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            Assert.That(sessionA.MissCount, Is.EqualTo(1));
        }
    }
}
