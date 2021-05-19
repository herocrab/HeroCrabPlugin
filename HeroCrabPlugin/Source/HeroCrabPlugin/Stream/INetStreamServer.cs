// ReSharper disable once CheckNamespace

public interface INetStreamServer : INetStream
{
    void KickAll();
    NetSessionServer CreateSession(INetSublayer netSublayer);
    INetElement CreateElement(string name, uint assetId, uint authorId, bool isEnabled);
    void DeleteElement(INetElement element);
    bool FindSession(uint id, out INetSession session);
}
