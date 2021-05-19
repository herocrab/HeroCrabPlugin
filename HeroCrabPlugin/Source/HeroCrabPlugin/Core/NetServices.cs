using System.Collections.Generic;
// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBeMadeStatic.Global

public class NetServices
{
    public int Count => Services.Count;
    public static readonly NetServices Registry = new NetServices();

    private static readonly Dictionary<string, object> Services = new Dictionary<string, object>();

    public void Clear() => Services.Clear();

    public T Get<T>()
    {
        var key = typeof(T).Name;
        if (!Services.ContainsKey(key)) {
            return default;
        }

        return (T) Services[key];
    }

    public void Add<T>(T service)
    {
        var key = service.GetType().Name;
        if (Services.ContainsKey(key)) {
            Services.Remove(key);
        }

        Services.Add(key, service);
    }

    public void Remove<T>()
    {
        var key = typeof(T).Name;
        if (!Services.ContainsKey(key)) {
            return;
        }

        Services.Remove(key);
    }
}
