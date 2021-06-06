using System;
using FlaxEngine;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream; // ReSharper disable MemberCanBePrivate.Global

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Network script for single-class processing on both client and server; use overrides.
    /// </summary>
    public class NetScript : Script
    {
        /// <summary>
        /// HeroCrabPlugin network server.
        /// </summary>
        protected INetStreamServer Server { get; set; }

        /// <summary>
        /// HeroCrabPlugin network element assigned to this script.
        /// </summary>
        protected INetElement Element { get; set; }

        private Action _start;
        private Action _enable;
        private Action _update;
        private Action _fixedUpdate;
        private Action _disable;
        private Action _destroy;

        /// <inheritdoc />
        public override void OnStart()
        {
            if (Element.IsClient) {
                _start = OnStartClient;
                _enable = OnEnableClient;
                _update = OnUpdateClient;
                _fixedUpdate = OnFixedUpdateClient;
                _disable = OnDisableClient;
                _destroy = OnDestroyClient;
            }
            else {
                _start = OnStartServer;
                _enable = OnEnableServer;
                _update = OnUpdateServer;
                _fixedUpdate = OnFixedUpdateServer;
                _disable = OnDisableServer;
                _destroy = OnDestroyServer;
            }

            _start.Invoke();
        }

        /// <inheritdoc/>
        public override void OnEnable() => _enable?.Invoke();

        /// <inheritdoc/>
        public override void OnDisable() => _disable?.Invoke();

        /// <inheritdoc/>
        public override void OnDestroy() => _destroy?.Invoke();

        /// <inheritdoc/>
        public override void OnUpdate() => _update.Invoke();

        /// <inheritdoc />
        public override void OnFixedUpdate() => _fixedUpdate?.Invoke();

        /// <summary>
        /// Start method called on the client.
        /// </summary>
        protected virtual void OnStartClient() { }

        /// <summary>
        /// Start method called on the server.
        /// </summary>
        protected virtual void OnStartServer() { }

        /// <summary>
        /// Enable method called on the client.
        /// </summary>
        protected virtual void OnEnableClient() { }

        /// <summary>
        /// Enable method called on the server.
        /// </summary>
        protected virtual void OnEnableServer() { }

        /// <summary>
        /// Update method called on the client.
        /// </summary>
        protected virtual void OnUpdateClient() { }

        /// <summary>
        /// Update method called on the server.
        /// </summary>
        protected virtual void OnUpdateServer() { }

        /// <summary>
        /// Fixed update method called on the client.
        /// </summary>
        protected virtual void OnFixedUpdateClient() { }

        /// <summary>
        /// Fixed update method called on the server.
        /// </summary>
        protected virtual void OnFixedUpdateServer() { }

        /// <summary>
        /// Disable method called on the client.
        /// </summary>
        protected virtual void OnDisableClient() { }

        /// <summary>
        /// Disable method called on the server.
        /// </summary>
        protected virtual void OnDisableServer() { }

        /// <summary>
        /// Destroy method called on the client.
        /// </summary>
        protected virtual void OnDestroyClient() { }

        /// <summary>
        /// Destroy method called on the server.
        /// </summary>
        protected virtual void OnDestroyServer() { }
    }
}
