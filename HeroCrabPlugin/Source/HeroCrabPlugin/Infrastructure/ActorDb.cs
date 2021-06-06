using System.Collections.Generic;
using FlaxEngine;

namespace Game.Infrastructure
{
    /// <summary>
    /// Asset database for spawning prefabs by asset id.
    /// </summary>
    public class ActorDb : Script
    {
        /// <summary>
        /// Map database for spawning the map, level, or world prefab.
        /// </summary>
        public Dictionary<string, Prefab> MapDb = new Dictionary<string, Prefab>();

        /// <summary>
        /// Prefab database for spawning assets; used by spawner.
        /// </summary>
        public Dictionary<AssetId, Prefab> PrefabDb = new Dictionary<AssetId, Prefab>();

        /// <summary>
        /// Asset id for prefab database lookup. These are using generic keys so they can be distributed with plugin.
        /// </summary>
        public enum AssetId : uint
        {
            #pragma warning disable 1591
            None,   // Use comments if using generic keys
            A001,   // VersionChecker
            A002,
            A003,
            A004,
            A005,
            #pragma warning restore 1591
        }
    }
}
