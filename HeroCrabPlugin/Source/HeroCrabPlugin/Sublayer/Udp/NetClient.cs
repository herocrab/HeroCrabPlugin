// Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab). Distributed under the MIT license.

using System;
using ENet;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer.Udp
{
    /// <summary>
    /// Network client (UDP).
    /// </summary>
    public class NetClient : NetHost, INetClient
    {
        /// <inheritdoc />
        public INetStreamClient Stream => _stream;

        private readonly Host _client;
        private readonly NetStreamClient _stream;
        private NetSublayer _sublayer;

        private Event _netEvent;
        private bool _polled;

        /// <inheritdoc />
        public uint RoundTripTime
        {
            get
            {
                return _sublayer?.RoundTripTime ?? 0;
            }
        }

        /// <summary>
        /// Create a new network client (UDP) given the configuration.
        /// </summary>
        /// <param name="netSettings">NetworkConfiguration</param>
        /// <returns></returns>
        public static INetClient Create(NetSettings netSettings )
        {
            if (netSettings == null) {
                netSettings = new NetSettings();
            }

            netSettings.UpdateBufferSettings(NetRole.Client);
            return new NetClient(netSettings);
        }

        private NetClient(NetSettings netSettings) : base (netSettings)
        {
            // Sub-layer
            Library.Initialize();
            _client = new Host();

            // Super-layer
            _stream = new NetStreamClient();
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

            Stream.KickAll();
            Stream.Clear();
            _client?.Flush();
            _client?.Dispose();
        }

        private void OnReceivePacket(Event netEvent)
        {
            _sublayer?.ReceivePacket(netEvent.ChannelID, netEvent.Packet);
        }

        private void OnDisconnectedEvent(Peer _)
        {
            if (_sublayer == null) {
                return;
            }

            Stream.DeleteSession(_sublayer);
        }

        private void OnConnectedEvent(Event netEvent)
        {
            _sublayer = NetSublayer.Create(netEvent.Peer);
            _sublayer.DisconnectCallback = OnDisconnectedEvent;
            _stream.CreateSession(_sublayer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Library.Deinitialize();
            _client?.Dispose();
        }
    }
}
