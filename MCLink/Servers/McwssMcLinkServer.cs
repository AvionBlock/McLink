using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WatsonWebsocket;

namespace MCLink.Servers
{
    public class McwssMcLinkServer : McLinkServer
    {
        private WatsonWsServer? _socket;

        public override void Start(int port)
        {
            CleanupSocket();
            _socket = new WatsonWsServer(new List<string> { "localhost", "127.0.0.1" }, port);
            _socket.ClientConnected += SocketOnClientConnected;
            _socket.MessageReceived += SocketOnMessageReceived;
            _socket.Start();
        }

        public override void Stop()
        {
            if (_socket?.IsListening ?? false) return;
            _socket?.Stop();
        }

        private void SocketOnClientConnected(object sender, ConnectionEventArgs e)
        {
            var eventPacket = new McwssEventStructure
            {
                body =
                {
                    eventName = "PlayerMessage"
                }
            };
            _socket?.SendAsync(e.Client.Guid, JsonSerializer.Serialize(eventPacket));
        }

        private void SocketOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.MessageType != WebSocketMessageType.Text) return;
            var data = Encoding.UTF8.GetString(e.Data);
            var packet = JsonSerializer.Deserialize<McwssPlayerMessageStructure>(data);
            if(packet == null) return;
            var rawtextData = JsonSerializer.Deserialize<RawTextStructure>(packet.body.message);
            Console.WriteLine(rawtextData);
        }

        private void CleanupSocket()
        {
            if (_socket == null) return;
            _socket.Dispose();
            _socket = null;
        }

        // ReSharper disable InconsistentNaming
        private class McwssEventStructure
        {
            public McwssEventBody body { get; set; } = new McwssEventBody();
        }

        private class McwssEventBody
        {
            public string eventName { get; set; } = string.Empty;
        }

        private class McwssPlayerMessageStructure
        {
            public McwssPlayerMessageBody body { get; set; } = new McwssPlayerMessageBody();
        }

        private class McwssPlayerMessageBody
        {
            public string message { get; set; } = string.Empty;
        }

        private class RawTextStructure
        {
            public RawTextMessage[] rawtext { get; set; } = Array.Empty<RawTextMessage>();
        }

        private class RawTextMessage
        {
            public string text { get; set; } = string.Empty;
        }
        // ReSharper enable InconsistentNaming
    }
}