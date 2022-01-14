// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System.Collections.Generic;
using HeroCrabPlugin.Core;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace HeroCrabPlugin.Element
{
    /// <summary>
    /// Network services container for registry singleton.
    /// </summary>
    public class NetElementMeta
    {
        private readonly Dictionary<string, object> _registry = new Dictionary<string, object>();

        /// <summary>
        /// Clear network services.
        /// </summary>
        public void Clear() => _registry.Clear();

        /// <summary>
        /// Get a network service from the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            var key = typeof(T).Name;
            if (!_registry.ContainsKey(key)) {
                return default;
            }

            return (T) _registry[key];
        }

        /// <summary>
        /// Add a network service to the singleton.
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(T service)
        {
            var key = service.GetType().Name;
            if (_registry.ContainsKey(key)) {
                _registry.Remove(key);
            }

            _registry.Add(key, service);
        }

        /// <summary>
        /// Remove a network service from the singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Remove<T>()
        {
            var key = typeof(T).Name;
            if (!_registry.ContainsKey(key)) {
                return;
            }

            _registry.Remove(key);
        }
    }
}
