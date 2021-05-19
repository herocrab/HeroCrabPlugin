// ReSharper disable once CheckNamespace

public interface INetStreamClient : INetStream
{
    NetSessionClient CreateSession(INetSublayer netSublayer);
    bool FindSession(out INetSession session);
}
