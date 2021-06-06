using System;
using FlaxEngine;
using FlaxEngine.GUI;
using HeroCrabPlugin.Core;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// GameClient Script.
    /// </summary>
    public class GameClient : Script
    {
        private INetClient _catalogClient;
        private INetClient _gameClient;

        /// <inheritdoc/>
        public override void OnStart()
        {
            InitializeGameClient();
            InitializeCatalogClient();
        }

        private void InitializeCatalogClient()
        {
            throw new NotImplementedException();
        }

        private void InitializeGameClient()
        {
            var exitButton = ((UIControl)Actor.FindActor("ExitButton")).Get<Button>();
            exitButton.ButtonClicked += delegate {Engine.RequestExit();};
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            // Here you can add code that needs to be called when script is enabled (eg. register for events)
        }

        /// <inheritdoc/>
        public override void OnDestroy()
        {
            _gameClient?.Stop();
            _catalogClient?.Stop();
        }

        /// <inheritdoc/>
        public override void OnFixedUpdate()
        {
            var time = Time.GameTime;
            _catalogClient?.Process(time);
            _gameClient?.Process(time);
        }
    }
}
