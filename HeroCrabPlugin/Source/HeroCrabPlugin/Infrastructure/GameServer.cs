using FlaxEngine;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Sublayer.Udp;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Game server for hosting live games.
    /// </summary>
    public class GameServer : Script
    {
        private INetServer _gameServer;
        private INetClient _registerClient;
        private INetElement _registration;

        /// <inheritdoc />
        public override void OnStart()
        {
            InitializeGameServer();
            InitializeRegisterClient();
        }

        /// <inheritdoc />
        public override void OnFixedUpdate()
        {
            var time = Time.GameTime;
            _gameServer.Process(time);
            _registerClient.Process(time);
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _registerClient?.Stop();
            _gameServer?.Stop();;
        }

        private void InitializeRegisterClient()
        {
            var config = NetBootStrap.Config;

            var settings = new NetSettings(NetRole.Client);
            _registerClient = NetClient.Create(settings);
            _registerClient.LogWrite += OnLogWrite;
            _registerClient.Stream.ElementCreated += OnRegistrationCreated;

            _registerClient.Start(config.RegisterAddress, config.RegisterPort);
        }

        private void OnRegistrationCreated(INetElement element)
        {
            var config = NetBootStrap.Config;

            Debug.Log($"[SERVER] Registering game server: {config.ServerName}.");

            _registration = element;
            _registration.GetString("Version").Set(config.Version);
            _registration.GetString("Name").Set(config.ServerName);
            _registration.GetString("Address").Set(config.ServerAddress);
            _registration.GetUShort("Port").Set(config.ServerPort);
            _registration.GetString("Map").Set(config.ServerMap);
            _registration.GetUShort("CurrentPlayers").Set((ushort)_gameServer.Stream.SessionCount);
            _registration.GetUShort("MaxPlayers").Set(config.MaxConnections);
        }

        private void InitializeGameServer()
        {
            var config = NetBootStrap.Config;
            var settings = new NetSettings(NetRole.Server, maxConnections: config.MaxConnections);
            _gameServer = NetServer.Create(settings);
            _gameServer.LogWrite += OnLogWrite;
            _gameServer.Stream.SessionConnected += OnClientConnected;

            // TODO setup asset DB in Flax
            // var assetDb = Entity.Scene.Entities.FirstOrDefault(a => a.Name == "AssetDb")?.Get<AssetDb>();
            // if (assetDb == null) {
            //     throw new NullReferenceException("AssetDb not found in scene!");
            // }

            // TODO setup spawner in Flax
            //var serverSpawner = new NetSpawner(assetDb, _gameServer.Stream);
            //Entity.Add(serverSpawner);

            _gameServer.Start(config.ServerAddress, config.ServerPort);
        }

        private void OnClientConnected(INetSession session)
        {
            _registration.GetUShort("CurrentPlayers").Set((ushort)_gameServer.Stream.SessionCount);

            // TODO move into loading
            // Move the session into the Load stream group
            //session.StreamGroup = NetStreamGroup.Load;

            // Deferred elements are not enabled by default, this is manually enabled by a script in Start method
            //_gameServer.Stream.CreateElement("LoadHandler", (uint) AssetDb.AssetId.LoadHandler, session.Id, false);
        }

        private void OnLogWrite(string message)
        {
            Debug.Log("[SERVER]" + message);
        }
    }
}
