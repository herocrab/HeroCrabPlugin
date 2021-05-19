﻿using System;
using System.Collections.Generic;
using System.Linq;
using ENet;
using HeroCrab.Network.Stream;
using HeroCrabPlugin.Core;
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Sublayer.Udp
{
    public class NetServer : NetHost, INetServer
    {
        public INetStreamServer Stream { get; }

        private readonly Host _server;
        private readonly SortedDictionary<uint, NetSublayer> _connections;

        private Event _netEvent;
        private bool _polled;
        private ushort _port;

        public static INetServer Create(NetConfig netConfig)
        {
            if (netConfig == null) {
                netConfig = new NetConfig(NetRole.Server);
            }
            return new NetServer(netConfig);
        }

        private NetServer(NetConfig netConfig) : base (netConfig)
        {
            // Sub-layer
            Library.Initialize();
            _server = new Host();

            _connections = new SortedDictionary<uint, NetSublayer>();

            // Super-layer
            Stream = new NetStreamServer();
        }

        public void Process(float time)
        {
            if (!_server.IsSet) {
                return;
            }

            _polled = false;

            while (!_polled) {

                if (_server.CheckEvents(out _netEvent) <= 0) {
                    if (_server.Service(0, out _netEvent) <= 0) {
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
                        OnMalformedEvent(_netEvent);
                        break;
                }
            }

            Stream.Process(time);
        }

        public void Start(string ipAddress, ushort port)
        {
            try {
                _port = port;

                var address = new Address();
                address.SetIP(ipAddress);
                address.Port = port;

                if (_server.IsSet) {
                    NetLogger.Write(NetLogger.LoggingGroup.Status, this, $"Server is already running on port {port}..." );
                    return;
                }

                NetLogger.Write(NetLogger.LoggingGroup.Status, this, $"[START] Server on port {port}..." );
                _server.Create(address, NetConfig.MaxConnections, 2);
            }
            catch (Exception) {
                NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                    $"[ERROR] Server could not be started on port {port}, is it something else running?");
            }
        }

        public void Stop()
        {
            if (!_server.IsSet) {
                return;
            }

            NetLogger.Write(NetLogger.LoggingGroup.Status, this, $"[STOP] Server on port {_port}..." );

            _server?.PreventConnections(true);

            KickAll();

            _server?.Flush();
            _server?.Dispose();
        }

        public void KickAll()
        {
            foreach (var sublayer in _connections.Values.ToArray()) {
                sublayer.Disconnect();
                Stream.DeleteSession(sublayer);
            }

            _connections.Clear();
        }

        private void OnReceivePacket(Event netEvent)
        {
            if (_connections.ContainsKey(netEvent.Peer.ID)) {
                _connections[netEvent.Peer.ID].ReceivePacket(netEvent.ChannelID, netEvent.Packet);
            }
        }

        private void OnConnectedEvent(Event netEvent)
        {
            var sublayer = NetSublayer.Create(netEvent.Peer);
            sublayer.DisconnectCallback = OnDisconnectedEvent;

            Stream.CreateSession(sublayer);

            _connections.Add(netEvent.Peer.ID, sublayer);
            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Server added new session ({sublayer.Id}) -> {netEvent.Peer.IP}:{netEvent.Peer.Port}");
        }

        private void OnDisconnectedEvent(Peer peer)
        {
            if (!_connections.ContainsKey(peer.ID)) {
                return;
            }

            var sublayer = _connections[peer.ID];

            NetLogger.Write(NetLogger.LoggingGroup.Session,this,
                $"Server removed session ({sublayer.Id}) -> {peer.IP}:{peer.Port}");

            Stream.DeleteSession(sublayer);
            _connections.Remove(peer.ID);
        }

        private void OnMalformedEvent(Event netEvent)
        {
            NetLogger.Write(NetLogger.LoggingGroup.Error,this,
                "[ERROR] Received an invalid NetEvent type. Disconnecting host.");

            netEvent.Peer.DisconnectNow(0);
            netEvent.Packet.Dispose();
        }

        public void Dispose()
        {
            Library.Deinitialize();
            _server?.Dispose();
        }
    }
}
