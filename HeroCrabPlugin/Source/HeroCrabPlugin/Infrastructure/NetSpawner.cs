using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine;
using Game.Infrastructure;
using HeroCrabPlugin.Element;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// Network spawner is responsible for spawning all prefabs on the server and client.
    /// </summary>
    public class NetSpawner : Script
    {
        private Dictionary<INetElement, Actor> _actors;
        private ActorDb _actorDb;
        private INetStream _stream;

        /// <inheritdoc />
        public override void OnStart()
        {
            InitializeNetSpawner();
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _stream.ElementCreated -= Spawn;
            _stream.ElementDeleted -= Despawn;

            base.OnDestroy();
        }

        /// <summary>
        /// Attach the network stream to this spawner.
        /// </summary>
        /// <param name="stream"></param>
        public void AttachStream(INetStream stream)
        {
            _stream = stream;
            _stream.ElementCreated += Spawn;
            _stream.ElementDeleted += Despawn;

            Clear();
        }

        /// <summary>
        /// Clear the spawner of all instantiated objects and destroy them.
        /// </summary>
        public void Clear()
        {
            _actors.Clear();
            foreach (var child in Actor.Children.ToArray()) {
                Destroy(child);
            }
        }

        private void Despawn(INetElement element)
        {
            if (!_actors.ContainsKey(element)) {
                return;
            }

            var actor = _actors[element];
            actor.Parent = null;

            _actors.Remove(element);
            Destroy(actor);
        }

        private void Spawn(INetElement element)
        {
            ActorDb.AssetId assetId;
            try {
                assetId = (ActorDb.AssetId) element.Description.AssetId;
            }
            catch {
                throw new InvalidCastException("Asset id could not be found in ActorDb.");
            }

            Prefab prefab;
            if (_actorDb.PrefabDb.ContainsKey(assetId)) {
                prefab = _actorDb.PrefabDb[assetId];
            }
            else {
                throw new IndexOutOfRangeException("Asset id not found in ActorDb.");
            }

            var serverStream = element.IsClient ? null : _stream as NetStreamServer;

            var actor = PrefabManager.SpawnPrefab(prefab);
            actor.Name = element.Description.Name;

            var script = actor.GetScript<NetScript>();
            script.Server = serverStream;
            script.Element = element;

            _actors.Add(element, actor);
            actor.Parent = Actor;
        }

        private void InitializeNetSpawner()
        {
            _actors = new Dictionary<INetElement, Actor>();
            _actorDb = Actor.GetScript<ActorDb>();

            if (_actorDb == null) {
                throw new NullReferenceException("ActorDb could not be found by network spawner.");
            }
        }
    }
}
