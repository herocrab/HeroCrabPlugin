// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.
using System.Collections.Generic;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace HeroCrabPlugin.Core
{
    /// <summary>
    /// Network services container for registry singleton.
    /// </summary>
    public class NetServices
    {
        /// <summary>
        /// Count of registered network services.
        /// </summary>
        public int Count => Services.Count;

        /// <summary>
        /// Registry of classes.
        /// </summary>
        public static readonly NetServices Registry = new NetServices();

        private static readonly Dictionary<string, object> Services = new Dictionary<string, object>();

        /// <summary>
        /// Clear network services.
        /// </summary>
        public void Clear() => Services.Clear();

        /// <summary>
        /// Get a network service from the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            var key = typeof(T).Name;
            if (!Services.ContainsKey(key)) {
                return default;
            }

            return (T) Services[key];
        }

        /// <summary>
        /// Add a network service to the singleton.
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(T service)
        {
            var key = service.GetType().Name;
            if (Services.ContainsKey(key)) {
                Services.Remove(key);
            }

            Services.Add(key, service);
        }

        /// <summary>
        /// Remove a network service from the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Remove<T>()
        {
            var key = typeof(T).Name;
            if (!Services.ContainsKey(key)) {
                return;
            }

            Services.Remove(key);
        }
    }
}
