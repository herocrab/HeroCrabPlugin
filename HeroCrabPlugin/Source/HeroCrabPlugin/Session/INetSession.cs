using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Session
{
    public interface INetSession
    {
        uint Id { get; }
        string Ip { get; }
        uint RxCount { get; }
        uint TxCount { get; }
        uint MissCount { get; }
        uint DupeCount { get; }
        NetStreamGroup StreamGroup { get; set; }
        void Disconnect();
    }
}
