using System;
using ENet;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Session;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Network client (UDP).
    /// </summary>
    public class NetClient : NetHost, INetClient
    {
        /// <inheritdoc />
        public INetStreamClient Stream { get; }

        private readonly Host _client;
        private NetSublayer _sublayer;
        private NetSessionClient _session;

        private Event _netEvent;
        private bool _polled;

        /// <summary>
        /// Create a new network client (UDP) given the configuration.
        /// </summary>
        /// <param name="netConfig">NetworkConfiguration</param>
        /// <returns></returns>
        public static INetClient Create(NetConfig netConfig )
        {
            if (netConfig == null) {
                netConfig = new NetConfig(NetRole.Client);
            }
            return new NetClient(netConfig);
        }

        private NetClient(NetConfig netConfig) : base (netConfig)
        {
            // Sub-layer
            Library.Initialize();
            _client = new Host();

            // Super-layer
            Stream = new NetStreamClient();
        }

        /// <inheritdoc />
        public void Process(float time)
        {
            if (!_client.IsSet) {
                return;
            }

            _polled = false;

            while (!_polled) {

                if (_client.CheckEvents(out _netEvent) <= 0) {
                    if (_client.Service(0, out _netEvent) <= 0) {
                        break;
                    }

                    _polled = true;
                }

                switch (_netEvent.Type) {
                    case EventType.None:
                        break;

                    case EventType.Connect:
                        OnConnectedEvent(_netEvent);
                        break;

                    case EventType.Disconnect:
                        OnDisconnectedEvent(_netEvent.Peer);
                        break;

                    case EventType.Timeout:
                        OnDisconnectedEvent(_netEvent.Peer);
                        break;

                    case EventType.Receive:
                        OnReceivePacket(_netEvent);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Stream.Process(time);
        }

        /// <inheritdoc />
        public void Start(string ipAddress, ushort port)
        {
            try {
                var address = new Address();
                address.SetHost(ipAddress);
                address.Port = port;

                if (_client.IsSet) {
                    NetLogger.Write(NetLogger.LoggingGroup.Status, this, "Client is already running..." );
                    return;
                }

                NetLogger.Write(NetLogger.LoggingGroup.Status, this, $"[START] Client connecting to {ipAddress}:{port}..." );
                _client.Create(1, 2);
                _client.Connect(address, 2);
            }
            catch (Exception) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this, $"[ERROR] Client could not connect to {ipAddress}:{port}");
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!_client.IsSet) {
                return;
            }

            NetLogger.Write(NetLogger.LoggingGroup.Status, this, "[STOP] Client is stopping..." );

            _session?.Disconnect();
            _client?.Flush();
            _client?.Dispose();
            Stream.Clear();
        }

        private void OnReceivePacket(Event netEvent)
        {
            _sublayer?.ReceivePacket(netEvent.ChannelID, netEvent.Packet);
        }

        private void OnDisconnectedEvent(Peer _)
        {
            Stream.DeleteSession(_sublayer);
        }

        private void OnConnectedEvent(Event netEvent)
        {
            _sublayer = NetSublayer.Create(netEvent.Peer);
            _sublayer.DisconnectCallback = OnDisconnectedEvent;

            _session = Stream.CreateSession(_sublayer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Library.Deinitialize();
            _client?.Dispose();
        }
    }
}
