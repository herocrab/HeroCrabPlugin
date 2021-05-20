using HeroCrab.Network.Stream;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;
using Moq;
using NUnit.Framework;

namespace HeroCrabPluginTestsUnit.Stream
{
    [TestFixture]
    public class NetStreamServerTests
    {
        [SetUp]
        public void SetUp()
        {
            NetServices.Registry.Clear();
            NetServices.Registry.Add(new NetConfig(NetRole.Server));
            NetServices.Registry.Add(new NetLogger(new NetLoggerBuffer(1000)));
        }

        [Test]
        public void Process_ProcessGameTickRateTimes_VerifyTxCountIsCorrect()
        {
            var netConfiguration = NetServices.Registry.Get<NetConfig>();

            var networkStream = new NetStreamServer();

            for (var i = 0; i < (int)netConfiguration.GameTickRate; i++) {
                networkStream.Process(i);
            }
        }

        [Test]
        public void CreateSession_CreateMaxPlusOneSessions_VerifySessionIdRollsOverToOneNotZero()
        {
            uint sentId = 0;
            void SendId(uint id)
            {
                sentId = id;
            }

            var networkStream = new NetStreamServer();
            networkStream.SetMaxSessionId();

            var fakeSublayerLast = new Mock<INetSublayer>();
            fakeSublayerLast.SetupAllProperties();
            fakeSublayerLast.Object.Id = 0;
            fakeSublayerLast.Setup(a => a.SendId(It.IsAny<uint>())).Callback<uint>(SendId);

            networkStream.CreateSession(fakeSublayerLast.Object);

            Assert.That(sentId, Is.EqualTo(1));
        }

        [Test]
        public void AddSession_AddTwoSessions_VerifySessionCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = 2;

            var networkStream = new NetStreamServer();

            networkStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback(1);

            networkStream.CreateSession(fakeSublayerB.Object);
            fakeSublayerA.Object.ReceiveIdCallback(2);

            Assert.That(networkStream.SessionCount, Is.EqualTo(2));
        }

        [Test]
        public void AddSession_AddThreeSessionsRemoveTwo_VerifySessionCountIsOne()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1; // Normally assigned by server stream

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = 2;

            var fakeSublayerC = new Mock<INetSublayer>();
            fakeSublayerC.SetupAllProperties();
            fakeSublayerC.Object.Id = 3;

            var serverStream = new NetStreamServer();

            // Emulate session assignment
            serverStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback.Invoke(1);

            serverStream.CreateSession(fakeSublayerB.Object);
            fakeSublayerB.Object.ReceiveIdCallback.Invoke(2);

            serverStream.CreateSession(fakeSublayerC.Object);
            fakeSublayerC.Object.ReceiveIdCallback.Invoke(3);

            serverStream.DeleteSession(fakeSublayerA.Object);
            serverStream.DeleteSession(fakeSublayerB.Object);

            Assert.That(serverStream.SessionCount, Is.EqualTo(1));
        }

        [Test]
        public void RemoveSession_RemoveSessionThatDoesNotExist_VerifyNoException()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = uint.MaxValue;

            var networkStream = new NetStreamServer();

            networkStream.CreateSession(fakeSublayerA.Object);
            Assert.DoesNotThrow(() => networkStream.DeleteSession(fakeSublayerB.Object));
        }

        [Test]
        public void KickAll_AddTwoSessionsAndKickAll_VerifyDisconnectCalledOnAllSessions()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = 2;

            var networkStream = new NetStreamServer();
            networkStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback(1);

            networkStream.CreateSession(fakeSublayerB.Object);
            fakeSublayerB.Object.ReceiveIdCallback(2);

            networkStream.KickAll();

            fakeSublayerA.Verify(a => a.Disconnect(), Times.Once);
            fakeSublayerB.Verify(a => a.Disconnect(), Times.Once);
        }

        [Test]
        public void Receive_AddTwoSessionsReceiveTwoPackets_VerifyRxCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = 2;

            var networkStream = new NetStreamServer();
            networkStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback.Invoke(1);

            networkStream.CreateSession(fakeSublayerB.Object);
            fakeSublayerB.Object.ReceiveIdCallback.Invoke(2);

            var rxQueue = new NetByteQueue();
            rxQueue.WriteByte((byte)NetSession.InnerTypeCode.Input);
            rxQueue.WriteInt(0); // Count

            // Below values emulate an "input" packet of type code 4, with 0 length
            fakeSublayerA.Object.ReceiveDataCallback?.Invoke(rxQueue.ToBytes());
            fakeSublayerB.Object.ReceiveDataCallback?.Invoke(rxQueue.ToBytes());

            Assert.That(networkStream.SessionCount, Is.EqualTo(2));
        }

        [Test]
        public void Create_CreateThreeElementsDeleteOne_VerifyElementCountIsTwo()
        {
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;

            var fakeSublayerB = new Mock<INetSublayer>();
            fakeSublayerB.SetupAllProperties();
            fakeSublayerB.Object.Id = 2;

            var networkStream = new NetStreamServer();
            networkStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback.Invoke(1);

            networkStream.CreateSession(fakeSublayerB.Object);
            fakeSublayerB.Object.ReceiveIdCallback.Invoke(2);

            networkStream.CreateElement("ElementA", 0, 1);
            networkStream.CreateElement("ElementB", 1, 2);
            var elementC = networkStream.CreateElement("ElementC", 2, 2);

            networkStream.DeleteElement(elementC);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                networkStream.Process(i);
            }

            Assert.That(networkStream.ElementCount, Is.EqualTo(2));
        }

        [Test]
        public void Delete_TryToDeleteAnElementThatDoesNotExist_NoErrorIsThrown()
        {
            var networkStream = new NetStreamServer();
            var nonExistentElement = new NetElement(new NetElementDesc(0, "Test", 0, 0));

            Assert.DoesNotThrow(() => networkStream.DeleteElement(nonExistentElement));
        }

        [Test]
        public void Process_CreateServerAndElementsThenProcess_VerifySubLayerSendIsCalledAndIsReliableAndPresent()
        {
            var serverSendIsReliable = false;
            var serverSendBytes = new byte[0];
            void Send(byte[] bytes, bool isReliable)
            {
                serverSendIsReliable = isReliable;
                serverSendBytes = bytes;
            }

            var serverStream = new NetStreamServer();
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;
            fakeSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(Send);

            serverStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback.Invoke(1);
            serverStream.CreateElement("ElementA", 0);
            serverStream.CreateElement("ElementB", 1, 1);

            serverStream.Process(0);

            Assert.That(serverSendIsReliable, Is.True);
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Process_CreateServerAndElementsThenProcess_VerifySubLayerSendIsCalledOnceAndIsReliableIsTrue()
        {
            var serverSendCount = 0;
            var serverSendBytes = new byte[0];
            var serverSendReliable = false;
            void Send(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                serverSendReliable = isReliable;
            }

            var serverStream = new NetStreamServer();
            var fakeSublayerA = new Mock<INetSublayer>();
            fakeSublayerA.SetupAllProperties();
            fakeSublayerA.Object.Id = 1;
            fakeSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(Send);

            serverStream.CreateSession(fakeSublayerA.Object);
            fakeSublayerA.Object.ReceiveIdCallback.Invoke(1);
            serverStream.CreateElement("ElementA", 0);
            serverStream.CreateElement("ElementB", 1, 1);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
            }

            Assert.That(serverSendCount, Is.EqualTo(1));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(serverSendReliable, Is.True);
        }

        [Test]
        public void Process_CreateServerAndElementsThenProcessAndReceive_VerifyAccuracyOfReceivedBytes()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendCount = 0;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);

            serverStream.CreateElement("ElementA", 0);
            serverStream.CreateElement("ElementB", 1);
            serverStream.CreateElement("ElementC", 2);
            serverStream.CreateElement("ElementD", 3);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendCount, Is.EqualTo(1));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(4));
        }

        [Test]
        public void Process_CreateServerAndElementsThenProcessDeleteTwoThenProcessReceive_VerifyAccuracyOfReceivedElements()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendCount = 0;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            var session = serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0);
            var elementB = serverStream.CreateElement("ElementB", 1);
            serverStream.CreateElement("ElementC", 2);
            serverStream.CreateElement("ElementD", 3);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            serverStream.DeleteElement(elementA);
            serverStream.DeleteElement(elementB);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendCount, Is.EqualTo(2));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(2));
            Assert.That(session.TxCount, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Process_CreateServerAndDeferredElementsEnableOnlyOneThenProcessReceive_VerifyAccuracyOfReceivedElements()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendCount = 0;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            var session = serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            serverStream.CreateElement("ElementA", 0);

            serverStream.CreateElement("ElementB", 1, 0, false);
            serverStream.CreateElement("ElementC", 2, 0, false);
            serverStream.CreateElement("ElementD", 3, 0, false);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendCount, Is.EqualTo(1));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(1));
            Assert.That(session.TxCount, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Process_CreateServerAndElementsThenProcessAddTwoThenReceive_VerifyAccuracyOfReceivedElements()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendReliableCount = 0;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendReliableCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0);
            var elementAStrengthSetter = elementA.AddByte("Strength", false, null);
            elementAStrengthSetter.Set(1);
            elementAStrengthSetter.Set(2);

            elementA.AddString("Name", false, null);

            serverStream.CreateElement("ElementB", 1);
            serverStream.CreateElement("ElementC", 2);
            serverStream.CreateElement("ElementD", 3);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            serverStream.CreateElement("ElementE", 4);
            serverStream.CreateElement("ElementF", 5);

            // Emulate game tick
            for (var i = 100; i < 200; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendReliableCount, Is.EqualTo(2));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(6));
        }

        [Test]
        public void Process_CreateServerAndElementsOneInputElementVerifyInputNotStreamed_VerifyInputElementIsNotStreamed()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendReliableCount = 0;
            var serverSendReliable = false;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendReliableCount++;
                serverSendReliable = isReliable;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0, 1);
            var elementAStrengthSetter = elementA.AddByte("Strength", false, null);
            elementAStrengthSetter.Set(1);
            elementAStrengthSetter.Set(2);

            var fakeServerSublayerB = new Mock<INetSublayer>();
            fakeServerSublayerB.SetupAllProperties();
            fakeServerSublayerB.Object.Id = 2;

            serverStream.CreateSession(fakeServerSublayerB.Object);
            fakeServerSublayerB.Object.ReceiveIdCallback.Invoke(2);

            // Ensure the author ID is not 0 or 1, because both sessions above share the index in NetSession
            var inputElement = serverStream.CreateElement("ElementB", 1, 2);
            inputElement.Filter.Recipient = 2;

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendReliableCount, Is.EqualTo(1));
            Assert.That(serverSendReliable, Is.True);
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(1));
        }

        [Test]
        public void Process_TwoServerSessionsOneTeamOneOneTeamTwo_VerifyTeamTwoReceivesTwoElementsTeamOneReceivesOneElement()
        {
            var clientStreamA = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;

            clientStreamA.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var clientStreamB = new NetStreamClient();
            var fakeClientSubLayerB = new Mock<INetSublayer>();
            fakeClientSubLayerB.SetupAllProperties();
            fakeClientSubLayerB.Object.Id = 2;

            clientStreamB.CreateSession(fakeClientSubLayerB.Object);
            fakeClientSubLayerB.Object.ReceiveIdCallback.Invoke(2);

            var serverSendCount = 0;
            var serverSendBytes = new byte[0];
            void ServerSendA(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            void ServerSendB(byte[] bytes, bool isReliable)
            {
                serverSendCount++;
                serverSendBytes = bytes;
                fakeClientSubLayerB.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;

            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSendA);

            var fakeServerSublayerB = new Mock<INetSublayer>();
            fakeServerSublayerB.SetupAllProperties();
            fakeServerSublayerB.Object.Id = 2;

            fakeServerSublayerB.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSendB);

            var serverSessionA = serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            serverSessionA.StreamGroup = NetStreamGroup.Team1;

            var serverSessionB = serverStream.CreateSession(fakeServerSublayerB.Object);
            fakeServerSublayerB.Object.ReceiveIdCallback.Invoke(2);
            serverSessionB.StreamGroup = NetStreamGroup.Team2;

            var elementA = serverStream.CreateElement("ElementA", 0);
            elementA.Filter.StreamGroup = NetStreamGroup.Team1;

            var elementB = serverStream.CreateElement("ElementB", 1);
            elementB.Filter.StreamGroup = NetStreamGroup.Team2 ;

            var elementC = serverStream.CreateElement("ElementC", 2);
            elementC.Filter.StreamGroup = NetStreamGroup.Team2;

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStreamA.Process(i);
            }

            Assert.That(serverSendCount, Is.EqualTo(2));
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStreamA.ElementCount, Is.EqualTo(1));
            Assert.That(clientStreamB.ElementCount, Is.EqualTo(2));
        }

        [Test]
        public void Process_CreateServerAndElementsAddOneReliableFieldToElement_VerifyIsReliable()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendIsReliable = false;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendIsReliable = isReliable;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0);
            elementA.AddString("Reliable Stat", true, null);

            // Emulate game tick
            for (var i = 0; i < 100; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(serverSendIsReliable, Is.True);
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(1));
        }

        [Test]
        public void Process_CreateServerAndElementsAttemptToApplyCreateForExistingElement_VerifyNoError()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            var clientSession = clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendIsReliable = false;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendIsReliable = isReliable;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0);
            elementA.AddString("Reliable Stat", true, null);

            serverStream.Process(1);
            clientStream.Process(1);

            fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(serverSendBytes);

            clientStream.Process(2);
            clientStream.Process(3);
            clientStream.Process(4);

            Assert.That(serverSendIsReliable, Is.True);
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(1));
            Assert.That(clientSession.DupeCount, Is.EqualTo(1));
        }

        [Test]
        public void Process_CreateServerAndElementsAttemptToApplyDeleteForExistingElement_VerifyNoError()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayerA = new Mock<INetSublayer>();
            fakeClientSubLayerA.SetupAllProperties();
            fakeClientSubLayerA.Object.Id = 1;
            var clientSession = clientStream.CreateSession(fakeClientSubLayerA.Object);
            fakeClientSubLayerA.Object.ReceiveIdCallback.Invoke(1);

            var serverSendIsReliable = false;
            var serverSendBytes = new byte[0];
            void ServerSend(byte[] bytes, bool isReliable)
            {
                serverSendIsReliable = isReliable;
                serverSendBytes = bytes;
                fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(bytes);
            }

            var serverStream = new NetStreamServer();
            var fakeServerSublayerA = new Mock<INetSublayer>();
            fakeServerSublayerA.SetupAllProperties();
            fakeServerSublayerA.Object.Id = 1;
            fakeServerSublayerA.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerSend);

            serverStream.CreateSession(fakeServerSublayerA.Object);
            fakeServerSublayerA.Object.ReceiveIdCallback.Invoke(1);
            var elementA = serverStream.CreateElement("ElementA", 0);
            elementA.AddString("Reliable Stat", true, null);

            serverStream.Process(1);
            clientStream.Process(1);

            serverStream.DeleteElement(elementA);

            serverStream.Process(2);
            clientStream.Process(2);

            serverStream.Process(3);
            clientStream.Process(3);

            fakeClientSubLayerA.Object.ReceiveDataCallback.Invoke(serverSendBytes);
            clientStream.Process(4);

            Assert.That(serverSendIsReliable, Is.True);
            Assert.That(serverSendBytes.Length, Is.GreaterThan(0));
            Assert.That(clientStream.ElementCount, Is.EqualTo(0));
            Assert.That(clientSession.DupeCount, Is.EqualTo(1));
        }

        [Test]
        public void Process_CreateServerAndInputElementsSendInput_VerifyRemoteCallbackCalled()
        {
            var characterSelectedCount = 0;
            var characterSelection = string.Empty;
            void ServerCharacterSelection(string selection)
            {
                characterSelectedCount++;
                characterSelection = selection;
            }

            void OnClientElementCreated(INetElement element)
            {
                var playerSelection = element.GetString("CharacterSelection");
                playerSelection?.Set("Warrior");
            }

            var clientStream = new NetStreamClient();
            clientStream.ElementCreated += OnClientElementCreated;
            var fakeClientSubLayer = new Mock<INetSublayer>();

            var serverStream = new NetStreamServer();
            var fakeServerSublayer = new Mock<INetSublayer>();

            fakeClientSubLayer.SetupAllProperties();
            fakeClientSubLayer.Object.Id = 1;
            fakeClientSubLayer.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerReceive);

            fakeServerSublayer.SetupAllProperties();
            fakeServerSublayer.Object.Id = 1;
            fakeServerSublayer.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ClientReceive);

            void ClientReceive(byte[] bytes, bool isReliable) => fakeClientSubLayer.Object.ReceiveDataCallback.Invoke(bytes);
            void ServerReceive(byte[] bytes, bool isReliable) => fakeServerSublayer.Object.ReceiveDataCallback.Invoke(bytes);

            clientStream.CreateSession(fakeClientSubLayer.Object);
            fakeClientSubLayer.Object.ReceiveIdCallback.Invoke(1);

            serverStream.CreateSession(fakeServerSublayer.Object);
            fakeServerSublayer.Object.ReceiveIdCallback.Invoke(1);

            var serverInputElement = serverStream.CreateElement("PlayerSelection", 0, 1);
            serverInputElement.Filter.Recipient = 1;
            serverInputElement.AddString("CharacterSelection", true, ServerCharacterSelection);

            // Emulate game tick
            for (var i = 0; i < 20; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            Assert.That(characterSelection, Is.EqualTo("Warrior"));
            Assert.That(characterSelectedCount, Is.EqualTo(1));
        }

        [Test]
        public void Process_CreateServerAndInputElementsApplySameInputFourTimes_VerifyDisconnectIsCalled()
        {
            var clientStream = new NetStreamClient();
            var fakeClientSubLayer = new Mock<INetSublayer>();
            fakeClientSubLayer.SetupAllProperties();
            fakeClientSubLayer.Object.Id = 1;

            var serverStream = new NetStreamServer();
            var fakeServerSublayer = new Mock<INetSublayer>();
            fakeServerSublayer.SetupAllProperties();
            fakeServerSublayer.Object.Id = 1;

            fakeClientSubLayer.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ServerReceive);

            fakeServerSublayer.Setup(x => x.Send(It.IsAny<byte[]>(), It.IsAny<bool>()))
                .Callback<byte[], bool>(ClientReceive);

            void ClientReceive(byte[] bytes, bool isReliable) => fakeClientSubLayer.Object.ReceiveDataCallback.Invoke(bytes);
            void ServerReceive(byte[] bytes, bool isReliable) => fakeServerSublayer.Object.ReceiveDataCallback.Invoke(bytes);

            clientStream.CreateSession(fakeClientSubLayer.Object);
            fakeClientSubLayer.Object.ReceiveIdCallback.Invoke(1);

            serverStream.CreateSession(fakeServerSublayer.Object);
            fakeServerSublayer.Object.ReceiveIdCallback.Invoke(1);

            var serverInputElement = serverStream.CreateElement("Element", 0, 1);
            serverInputElement.AddByte("Byte", true, null);

            // Emulate game tick
            for (var i = 0; i < 20; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            var rxQueue = new NetByteQueue();
            rxQueue.WriteByte(3);           // Type Code
            rxQueue.WriteInt(4);            // Element Count

            for (var i = 0; i < 4; i++) {
                rxQueue.WriteUInt(0);   // Element Index
                rxQueue.WriteInt(4);            // Field "Bytes"
                rxQueue.WriteInt(0);            // Field Count
            }

            // Emulate game tick
            for (var i = 0; i < 20; i++) {
                serverStream.Process(i);
                clientStream.Process(i);
            }

            fakeServerSublayer.Object.ReceiveDataCallback.Invoke(rxQueue.ToBytes());
            fakeServerSublayer.Verify(x => x.Disconnect(), Times.Once);
        }
    }
}
