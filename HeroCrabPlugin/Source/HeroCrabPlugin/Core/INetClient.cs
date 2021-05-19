// ReSharper disable once CheckNamespace

public interface INetClient : INetHost
{
    INetStreamClient Stream { get; }
}
