// ReSharper disable once CheckNamespace
public interface INetServer : INetHost
{
    INetStreamServer Stream { get; }
    void KickAll();
}
