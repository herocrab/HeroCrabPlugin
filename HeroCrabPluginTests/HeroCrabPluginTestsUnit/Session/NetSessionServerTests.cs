// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;
using Moq;
using NUnit.Framework; // ReSharper disable UnusedVariable

namespace HeroCrabPluginTestsUnit.Session
{
    [TestFixture]
    public class NetSessionServerTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetSettings());
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Send_ReceiveZeroBytesInSession_AssertThatErrorIsThrown()
        {
            var logMessage = string.Empty;

            void LogWrite(object sender, string message)
            {
                logMessage = message;
            }

            var netLogger = NetServices.Registry.Get<NetLogger>();
            netLogger.LogWrite += LogWrite;

            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var clientSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());

            Assert.That(logMessage.Contains("ERROR"));
        }

        [Test]
        public void Send_SendTwoPacketThroughSession_AssertSublayerSendIsCalledTwoTimesVerifyTxCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var elements = new SortedDictionary<uint, NetElement>
            {
                {0, new NetElement(new NetElementDesc(0, "Test1", 0, 0))},
                {1, new NetElement(new NetElementDesc(1, "Test2", 0, 0))}
            };

            var send = new SortedDictionary<uint, List<NetElement>>
            {
                {0, elements.Values.ToList()},
                {1, new List<NetElement>()}
            };

            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var sessionA = new NetSessionServer(fakeSublayerA.Object, send, exclude);
            sessionA.Send(0);

            elements.Add(2, new NetElement(new NetElementDesc(2, "Test3", 0, 0)));
            send[0] = elements.Values.ToList();
            sessionA.Send(1);

            fakeSublayerA.Verify(a => a.Send(It.IsAny<float>(),
                It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Exactly(2));

            Assert.That(sessionA.TxCount, Is.EqualTo(2));
        }

        [Test]
        public void Send_ModifyTwoExistingElements_AssertSublayerSendIsCalledThreeTimesVerifyTxCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var elements = new SortedDictionary<uint, NetElement>
            {
                {0, new NetElement(new NetElementDesc(0, "Test1", 0, 0))},
                {1, new NetElement(new NetElementDesc(1, "Test2", 0, 0))}
            };

            var send = new SortedDictionary<uint, List<NetElement>>
            {
                {0, elements.Values.ToList()},
                {1, new List<NetElement>()}
            };

            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var sessionA = new NetSessionServer(fakeSublayerA.Object, send, exclude);
            sessionA.Send(0);

            var testElement = new NetElement(new NetElementDesc(2, "Test3", 0, 0));
            var testSetter = testElement.AddByte("Byte", true);
            elements.Add(2, testElement);

            send[0] = elements.Values.ToList();
            sessionA.Send(1);

            testSetter.Set(byte.MaxValue);
            testElement.PrepareDelta();

            send[0] = elements.Values.ToList();
            sessionA.Send(2);

            fakeSublayerA.Verify(a => a.Send(It.IsAny<float>(),
                It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Exactly(3));

            Assert.That(sessionA.TxCount, Is.EqualTo(3));
        }


        [Test]
        public void Send_SendAPacketWithNoDataOnlyHeaders_NothingIsSent()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>
            {
                {0, new List<NetElement>()},
                {1, new List<NetElement>()}
            };

            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var sessionA = new NetSessionServer(fakeSublayerA.Object, send, exclude);
            sessionA.Send(0);

            fakeSublayerA.Verify(a => a.Send(It.IsAny<float>(),
                It.IsAny<byte[]>(), It.IsAny<bool>()), Times.Exactly(0));

            Assert.That(sessionA.TxCount, Is.EqualTo(0));
        }

        [Test]
        public void ReceivePacket_ServerReceivesMalformedInput_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            txQueue.WriteInt(1); // Input count
            txQueue.WriteByte(0); // Expects an int, this should throw an exception

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Exactly(1));
        }

        [Test]
        public void ReceivePacket_AttemptToOverwriteInputElementFourTimes_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var inputElement = new NetElement(new NetElementDesc(0, "Input", 0, 0));
            inputElement.AddByte("Byte", false);

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            txQueue.WriteInt(4); // Input count
            txQueue.WriteByte(0);
            txQueue.WriteByte(1);
            txQueue.WriteByte(2);
            txQueue.WriteByte(3);

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Exactly(1));
        }

        [Test]
        public void ReceivePacket_ServerReceivesPacketTooBig_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteBytes(new byte[512]); // 512 + 4 is the largest allowed through WriteBytes
            txQueue.WriteBytes(new byte[512]); // 512 + 4 is the largest allowed through WriteBytes
            txQueue.WriteBytes(new byte[512]); // 512 + 4 is the largest allowed through WriteBytes
            txQueue.WriteBytes(new byte[512]); // 512 + 4 is the largest allowed through WriteBytes

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Exactly(1));
        }

        [Test]
        public void ReceivePacket_ServerReceivesMalformedInputWrongTypeCode_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte(9);
            txQueue.WriteInt(1); // Input count
            txQueue.WriteByte(0); // Expects an int, this should throw an exception

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Exactly(1));
        }

        [Test]
        public void ReceivePacket_ServerReceivesMalformedInputInsideApplyInput_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            txQueue.WriteInt(1); // Input count
            txQueue.WriteInt(0);

            fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            fakeSublayerA.Verify(a => a.Disconnect(), Times.Exactly(1));
        }

        [Test]
        public void ReceivePacket_AttemptToBurstPacketRate_VerifyDisconnectIsCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            txQueue.WriteInt(0);

            for (float i = 0; i < 1.25; i += .01f) {
                serverSession.Process(i);
                fakeSublayerA.Object.ReceiveDataCallback.Invoke(txQueue.ToBytes());
            }

            fakeSublayerA.Verify(a => a.Disconnect(), Times.AtLeast(1));
            Assert.That(serverSession.RxCount, Is.GreaterThanOrEqualTo(100));
        }

        [Test]
        public void ReceivePacket_SustainPacketRateTestResetOfCounter_VerifyNDisconnectIsNotCalled()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var send = new SortedDictionary<uint, List<NetElement>>();
            var exclude = new SortedDictionary<uint, List<NetElement>>();
            var serverSession = new NetSessionServer(fakeSublayerA.Object, send, exclude);

            var txQueue = new NetByteQueue();
            txQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            txQueue.WriteInt(0);

            for (float i = 0; i < 1.25; i += .1f) {
                serverSession.Process(i);
            }

            fakeSublayerA.Verify(a => a.Disconnect(), Times.Never);
        }
    }
}
