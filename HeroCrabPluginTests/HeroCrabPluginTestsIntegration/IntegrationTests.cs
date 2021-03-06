using System.Collections.Generic;
using System.Threading;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer;
using HeroCrabPlugin.Sublayer.Replay;
using HeroCrabPlugin.Sublayer.Udp;
using NUnit.Framework; // ReSharper disable NotAccessedField.Local

[assembly:Apartment(ApartmentState.STA)]
namespace HeroCrabPluginTestsIntegration
{
    [TestFixture]
    public class NetServerTests
    {
        private INetServer _server;
        private INetClient _clientA;
        private INetClient _clientB;
        private INetSession _clientASession;
        private INetSession _clientBSession;

        private INetElement _serverElementA;
        private INetElement _serverElementB;
        private INetElement _serverElementC;
        private INetElement _serverElementD;
        private INetElement _clientInputElement;
        private INetElement _lastElement;

        private int _processCount;
        private int _serverSessionConnectedCount;
        private int _serverSessionDisconnectedCount;
        private int _clientSessionConnectedCount;
        private int _clientSessionDisconnectedCount;
        private int _serverInputElementStringChanged;
        private int _clientAInputElementStringChanged;
        private int _clientBInputElementStringChanged;

        [SetUp]
        public void Setup()
        {
            _server = NetServer.Create(new NetSettings());

            var clientSettings = new NetSettings();
            clientSettings.UpdateBufferSettings(NetRole.Client);

            _clientA = NetClient.Create(clientSettings);
            _clientB = NetClient.Create(clientSettings);
        }

        private void StartServer()
        {
            _server.Stream.SessionConnected += OnServerSessionConnected;
            _server.Stream.SessionDisconnected += OnServerSessionDisconnected;
            _server.Start("127.0.0.1", 42056);
        }

        private void StartClients()
        {
            _clientA.Stream.SessionConnected += OnClientSessionConnected;
            _clientB.Stream.SessionConnected += OnClientSessionConnected;

            _clientA.Stream.SessionDisconnected += OnClientSessionDisconnected;
            _clientB.Stream.SessionDisconnected += OnClientSessionDisconnected;

            _clientA.Stream.ElementCreated += OnClientElementCreated;
            _clientB.Stream.ElementCreated += OnClientElementCreated;

            _clientA.Start("127.0.0.1", 42056);
            _clientB.Start("127.0.0.1", 42056);
        }

        private void Process(uint count)
        {
            for (var i = 0; i < count; i++) {
                _server?.Process(_processCount);
                _clientA?.Process(_processCount);
                _clientB?.Process(_processCount);
                _processCount++;
            }
        }

        private void Reset()
        {
            _server?.Stop();
            _clientA?.Stop();
            _clientB?.Stop();

            _processCount = 0;
            _serverSessionConnectedCount = 0;
            _serverSessionDisconnectedCount = 0;
            _clientSessionConnectedCount = 0;
            _clientSessionDisconnectedCount = 0;
            _serverInputElementStringChanged = 0;
            _clientAInputElementStringChanged = 0;
            _clientBInputElementStringChanged = 0;
        }

        private void OnServerInputElementInputString(string value)
        {
            _serverInputElementStringChanged++;
        }

        private void OnClientAInputElementInputString(string value)
        {
            _clientAInputElementStringChanged++;
        }

        private void OnClientBInputElementInputString(string value)
        {
            _clientBInputElementStringChanged++;
        }

        private void OnServerSessionConnected(INetSession session)
        {
            _serverSessionConnectedCount++;

            // Cache the sessions
            if (session.Id == 1) {
                _clientASession = session;
            } else if (session.Id == 2) {
                _clientBSession = session;
            }

            // This reference is rotary
            _clientInputElement = _server.Stream.CreateElement($"Client{session.Id}", 0, session.Id);
            _clientInputElement.Filter.Recipient = session.Id;

            _clientInputElement.AddString("Input", true, OnServerInputElementInputString);
        }

        private void OnServerSessionDisconnected(INetSession session)
        {
            _serverSessionDisconnectedCount++;
        }

        private void OnClientSessionConnected(INetSession session)
        {
            _clientSessionConnectedCount++;
        }

        private void OnClientSessionDisconnected(INetSession session)
        {
            _clientSessionDisconnectedCount++;
        }

        private void OnClientElementCreated(INetElement element)
        {
            _lastElement = element;

            if (element.Description.AuthorId == 1) {
                element.SetActionString("Input", OnClientAInputElementInputString);
            }

            if (element.Description.AuthorId == 2) {
                element.SetActionString("Input", OnClientBInputElementInputString);
            }

            if (element.Description.AuthorId == 0) {
                return;
            }

            var inputSetter = element.GetString("Input");
            inputSetter.Set($"Element:{element.Description.Id}:{element.Description.AuthorId}A");

            if (element.Description.AuthorId == 2) {
                inputSetter.Set($"Element:{element.Description.Id}:{element.Description.AuthorId}B");
                inputSetter.Set($"Element:{element.Description.Id}:{element.Description.AuthorId}C");
                inputSetter.Set($"Element:{element.Description.Id}:{element.Description.AuthorId}D");
            }
        }

        private void AddServerElements()
        {
            _serverElementA = _server.Stream.CreateElement("ServerElementA", 0);
            _serverElementA.AddFloat("SomeFloat", false);

            _serverElementB = _server.Stream.CreateElement("ServerElementB", 0);
            _serverElementB.AddFloat("SomeFloat", false);

            _serverElementC = _server.Stream.CreateElement("ServerElementC", 0);
            _serverElementC.AddFloat("SomeFloat", false);

            _serverElementD = _server.Stream.CreateElement("ServerElementD", 0);
            _serverElementD.AddFloat("SomeFloat", false);
            _serverElementD.Filter.Exclude = 2;
        }

        private void DeleteServerElementA()
        {
            _serverElementA.Delete();
        }

        private void DeleteLastClientInputElement()
        {
            _clientInputElement.Delete();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndServerKickAllIntegrationTest()
        {
            StartServer();
            Process(100);
            StartClients();
            Process(100);

            _server.KickAll();

            Process(100);

            Assert.That(_serverSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_serverSessionDisconnectedCount, Is.EqualTo(2));
            Assert.That(_clientSessionDisconnectedCount, Is.EqualTo(2));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndServerStopOneClient()
        {
            StartServer();
            Process(100);
            StartClients();
            Process(100);

            _clientB.Stop();

            Process(100);

            Assert.That(_serverSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_serverSessionDisconnectedCount, Is.EqualTo(1));
            Assert.That(_clientSessionDisconnectedCount, Is.EqualTo(1));
            Assert.That(_server.Stream.SessionCount, Is.EqualTo(2));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndServerKickOneClientFromServer()
        {
            StartServer();
            Process(100);
            StartClients();
            Process(100);

            _clientBSession.Disconnect();

            Process(100);

            Assert.That(_serverSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_clientSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_serverSessionDisconnectedCount, Is.EqualTo(1));
            Assert.That(_clientSessionDisconnectedCount, Is.EqualTo(1));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndClientInputIntegrationTest()
        {
            Process(100);
            StartServer();
            Process(100);
            StartClients();
            Process(100);
            AddServerElements();
            Process(100);
            DeleteServerElementA();
            Process(100);
            DeleteLastClientInputElement();
            Process(100);

            Assert.That(_clientSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_serverSessionConnectedCount, Is.EqualTo(2));

            Assert.That(_server.Stream.ElementCount, Is.EqualTo(4));
            Assert.That(_clientA.Stream.ElementCount, Is.EqualTo(4)); //A, B, C, D
            Assert.That(_clientB.Stream.ElementCount, Is.EqualTo(2)); //B, C

            Assert.That(_serverInputElementStringChanged, Is.EqualTo(5));
            Assert.That(_clientAInputElementStringChanged, Is.EqualTo(1));
            Assert.That(_clientBInputElementStringChanged, Is.EqualTo(4));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndMaxConnectionTest()
        {
            _server = NetServer.Create(new NetSettings(maxConnections:1));

            StartServer();
            Process(100);
            StartClients();
            Process(100);

            Assert.That(_serverSessionConnectedCount, Is.EqualTo(1));
            Assert.That(_server.Stream.SessionCount, Is.EqualTo(2));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndTestAddingFieldsMoreThanOnce()
        {
            Process(100);
            StartServer();
            Process(100);
            StartClients();
            Process(100);
            AddServerElements();
            Process(100);

            Assert.That(_clientSessionConnectedCount, Is.EqualTo(2));
            Assert.That(_serverSessionConnectedCount, Is.EqualTo(2));

            Assert.That(_server.Stream.ElementCount, Is.EqualTo(6));
            Assert.That(_clientA.Stream.ElementCount, Is.EqualTo(5));
            Assert.That(_clientB.Stream.ElementCount, Is.EqualTo(4));

            Assert.That(_serverInputElementStringChanged, Is.EqualTo(5));
            Assert.That(_clientAInputElementStringChanged, Is.EqualTo(1));
            Assert.That(_clientBInputElementStringChanged, Is.EqualTo(4));

            // Disable element as to add more fields.
            _serverElementA.Enabled = false;
            Process(100);

            Assert.That(_server.Stream.ElementCount, Is.EqualTo(6));
            Assert.That(_clientA.Stream.ElementCount, Is.EqualTo(4));
            Assert.That(_clientB.Stream.ElementCount, Is.EqualTo(3));

            // Add more fields to the element
            _serverElementA.AddByte("SomethingAddedA", true);
            _serverElementA.AddByte("SomethingAddedB", true);
            _serverElementA.Enabled = true;

            Process(100);

            Assert.That(_serverElementA.FieldCount, Is.EqualTo(3));
            Assert.That(_lastElement.FieldCount, Is.EqualTo(3));

            Reset();
        }

        [Test, Apartment(ApartmentState.STA)]
        public void EndToEndReplayIntegrationTest()
        {
            const string firstMessage = "This is my first test message!";
            const string secondMessage = "This is my second test message!";

            var server = NetServer.Create(new NetSettings());
            server.Start("127.0.0.1", 42056);
            server.Stream.Recorder.Start(0);
            var messageElement = server.Stream.CreateElement("Test", 0);
            var messageField = messageElement.AddString("Message", true);

            for (int i = 0; i < 10; i++) {
                server.Process(i);
            }
            messageField.Set(firstMessage);

            for (int i = 10; i < 20; i++) {
                server.Process(i);
            }
            messageField.Set(secondMessage);

            for (int i = 20; i < 30; i++) {
                server.Process(i);
            }

            server.Stop();
            var replayBytes = server.Stream.Recorder.Bytes;

            var receivedMessages = new List<string>();
            void ReceiveMessage(string receivedMessage)
            {
                receivedMessages.Add(receivedMessage);
            }

            void ElementCreated(INetElement createdElement)
            {
                if (createdElement.Description.Name == "Test") {
                    var field = createdElement.SetActionString("Message", ReceiveMessage);
                }
            }

            var replay = NetReplay.Create();
            replay.Stream.ElementCreated += ElementCreated;
            replay.Play(0, replayBytes);

            for (int i = 0; i < 30; i++) {
                replay.Process(i);
            }

            replay.Stop();

            Assert.That(receivedMessages.Count, Is.EqualTo(2));
            Assert.That(receivedMessages[0], Is.EqualTo(firstMessage));
            Assert.That(receivedMessages[1], Is.EqualTo(secondMessage));
        }
    }
}
