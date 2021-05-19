using System.IO;
using FlaxEngine.Json;
// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

public class NetBootConfig
{
    public string Role { get; set; }
    public string RegisterAddress { get; set; }
    public string CatalogAddress { get; set; }
    public string ServerAddress { get; set; }
    public string ServerName { get; set; }
    public string ServerMap { get; set; }
    public ushort RegisterPort { get; set; }
    public ushort CatalogPort { get; set; }
    public ushort ServerPort { get; set; }
    public ushort MaxConnections { get; set; }
    public ushort MaxCatalogSize { get; set; }
    public ushort MaxLogSize { get; set; }

    public NetBootConfig(
        string role = "client",
        string registerAddress = "127.0.0.1",
        string catalogAddress = "127.0.0.1",
        string serverAddress = "127.0.0.1",
        string serverName = "HeroCrab.Network GameServer",
        string serverMap = "DemoMap",
        ushort registerPort = 42056,
        ushort catalogPort = 42057,
        ushort serverPort = 42058,
        ushort maxConnections = 100,
        ushort maxCatalogSize = 100,
        ushort maxLogSize = 1000)
    {
        Role = role;
        RegisterPort = registerPort;
        CatalogPort = catalogPort;
        ServerPort = serverPort;
        MaxConnections = maxConnections;
        MaxCatalogSize = maxCatalogSize;
        MaxLogSize = maxLogSize;
        RegisterAddress = registerAddress;
        CatalogAddress = catalogAddress;
        ServerAddress = serverAddress;
        ServerName = serverName;
        ServerMap = serverMap;
    }

    public static void Write(string filename)
    {
        var config = new NetBootConfig();
        var jsonString = JsonSerializer.Serialize(config, true);
        File.WriteAllText(filename, jsonString);
    }

    public static NetBootConfig Read(string filename)
    {
        var jsonString = File.ReadAllText(filename);
        var config = JsonSerializer.Deserialize<NetBootConfig>(jsonString);
        return config;
    }
}

