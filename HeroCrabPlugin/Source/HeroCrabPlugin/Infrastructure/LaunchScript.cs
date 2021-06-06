using System.Linq;
using FlaxEngine;
using HeroCrabPlugin.Core; // ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace HeroCrabPlugin.Infrastructure
{
    /// <summary>
    /// LaunchScript Script.
    /// </summary>
    public class LaunchScript : Script
    {
        /// <summary>
        /// Catalog server scene.
        /// </summary>
        public SceneReference Catalog;

        /// <summary>
        /// Server scene.
        /// </summary>
        public SceneReference Server;

        /// <summary>
        /// Client server scene.
        /// </summary>
        public SceneReference Client;

        /// <inheritdoc/>
        public override void OnStart()
        {
            InitializeHeroCrabPlugin();
            ToNetworkScene();
        }

        private void ToNetworkScene()
        {
            switch (NetBootStrap.Config.Role.ToLower()) {
                case "catalog":
                    Debug.Log("Transitioning to [CATALOG] scene.");
                    Level.ChangeSceneAsync(Catalog);
                    break;

                case "server":
                    Debug.Log("Transitioning to [SERVER] scene.");
                    Level.ChangeSceneAsync(Server);
                    break;

                case "client":
                    Debug.Log("Transitioning to [CLIENT] scene.");
                    Level.ChangeSceneAsync(Client);
                    break;

                default:
                    Debug.Log("Transitioning to [CLIENT] scene.");
                    Level.ChangeSceneAsync(Client);
                    break;
            }
        }

        private static void InitializeHeroCrabPlugin()
        {
            var configJson = Engine.CommandLine.Split(' ').First(a => a.Contains(".json"));

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (NetBootStrap.Initialize(configJson)) {
                    Debug.Log($"HeroCrabPlugin configuration .json found: {configJson}");
            }
            else {
                Debug.Log("No HeroCrabPlugin configuration .json found, using default client.");
            }
        }
    }
}
