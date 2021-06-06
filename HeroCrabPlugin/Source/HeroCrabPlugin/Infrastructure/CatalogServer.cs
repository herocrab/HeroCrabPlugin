using System.Collections.Generic;
using FlaxEngine;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;
using HeroCrabPlugin.Sublayer.Udp;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Catalog server for registering and retrieving live games.
    /// </summary>
    public class CatalogServer : Script
    {
        private SortedDictionary<uint, ServerAdvertisement> _advertisements;

        private INetServer _registerServer;
        private INetServer _catalogServer;

        /// <inheritdoc />
        public override void OnStart()
        {
            InitializeCatalogServer();
            InitializeRegisterServer();
        }

        /// <inheritdoc />
        public override void OnFixedUpdate()
        {
            var time = Time.GameTime;
            _registerServer.Process(time);
            _catalogServer.Process(time);
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _registerServer?.Stop();
            _catalogServer?.Stop();
        }

        private void InitializeRegisterServer()
        {
            var config = NetBootStrap.Config;
            var settings = new NetSettings(NetRole.Server, maxConnections: config.MaxConnections);
            _registerServer = NetServer.Create(settings);
            _registerServer.LogWrite += OnLogWrite;
            _registerServer.Stream.SessionConnected += OnRegistrationConnected;
            _registerServer.Stream.SessionDisconnected += OnRegistrationDisconnected;

            _registerServer.Start(config.RegisterAddress, config.RegisterPort);
        }

        private void OnRegistrationDisconnected(INetSession session)
        {
            if (!_advertisements.ContainsKey(session.Id)) {
                return;
            }

            var element = _advertisements[session.Id].Element;

            Debug.Log($"[CATALOG] Registration deleted for game server: {session.Id}:{session.Ip}.");

            // Delete this element from the catalog server; registration clean-up occurs automatically on author disconnect
            _catalogServer.Stream.DeleteElement(_advertisements[session.Id].Element);
            _advertisements.Remove(session.Id);
        }

        private void OnRegistrationConnected(INetSession session)
        {
            if (_advertisements.ContainsKey(session.Id)) {
                session.Disconnect();
                return;
            }

            var advertisement = new ServerAdvertisement(_catalogServer, session);
            var registration = new ServerRegistration(_registerServer, session, advertisement);

            Debug.Log($"[CATALOG] Registration created for game server: {session.Id}:{session.Ip}.");

            _advertisements.Add(session.Id, advertisement);
        }

        private void InitializeCatalogServer()
        {
            _advertisements = new SortedDictionary<uint, ServerAdvertisement>();

            var config = NetBootStrap.Config;
            var settings = new NetSettings(NetRole.Server, maxConnections: config.MaxConnections);
            _catalogServer = NetServer.Create(settings);
            _catalogServer.LogWrite += OnLogWrite;
            _catalogServer.Stream.SessionConnected += OnClientConnected;

            _catalogServer.Start(config.CatalogAddress, config.CatalogPort);
        }

        private void OnClientConnected(INetSession session)
        {
            session.StreamGroup = NetStreamGroup.Lobby;
        }

        private void OnLogWrite(string message)
        {
            Debug.Log("[CATALOG]" + message);
        }
    }
}
