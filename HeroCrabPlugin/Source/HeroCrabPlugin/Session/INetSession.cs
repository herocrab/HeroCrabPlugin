using HeroCrabPlugin.Stream;
// ReSharper disable UnusedMemberInSuper.Global

namespace HeroCrabPlugin.Session
{
    /// <summary>
    /// Network session.
    /// </summary>
    public interface INetSession
    {
        /// <summary>
        /// Id of the network session.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Ip address of the network session.
        /// </summary>
        string Ip { get; }

        /// <summary>
        /// Count of received packets.
        /// </summary>
        uint RxCount { get; }

        /// <summary>
        /// Count of transmitted packets.
        /// </summary>
        uint TxCount { get; }

        /// <summary>
        /// Count of packets received with invalid elements.
        /// </summary>
        uint MissCount { get; }

        /// <summary>
        /// Count of packets received with duplicate create or delete entries.
        /// </summary>
        uint DupeCount { get; }

        /// <summary>
        /// Stream group used when filtering which elements to send this session.
        /// </summary>
        NetStreamGroup StreamGroup { get; set; }

        /// <summary>
        /// Disconnect this session.
        /// </summary>
        void Disconnect();
    }
}
