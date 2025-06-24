using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WatsonWebsocket;

namespace MCLink.Servers.Mcwss
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
            try
            {
                var eventPacket = new McwssEventStructure
                {
                    body =
                    {
                        eventName = "PlayerMessage"
                    }
                };
                var ip = IPAddress.Parse(e.Client.Ip);
                e.Client.Metadata = new IPEndPoint(ip, e.Client.Port);
                _socket?.SendAsync(e.Client.Guid, JsonSerializer.Serialize(eventPacket));
            }
            finally
            { 
                _socket?.DisconnectClient(e.Client.Guid);
            }
        }

        private void SocketOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                if (e.MessageType != WebSocketMessageType.Text) return;
                var data = Encoding.UTF8.GetString(e.Data);
                var packet = JsonSerializer.Deserialize<McwssPlayerMessageStructure>(data);
                if (packet == null) return;
                var rawtextData = JsonSerializer.Deserialize<RawTextStructure>(packet.body.message);
                if (rawtextData == null) return;
                var textData = rawtextData.rawtext.FirstOrDefault()?.text;
                if (string.IsNullOrEmpty(textData)) return;

                Reader.SetSource(Encoding.UTF8.GetBytes(textData));
                if (!(e.Client.Metadata is IPEndPoint endPoint)) return;
                HandlePacket(Reader, endPoint);
            }
            finally
            {
                Reader.Clear();
            }
        }

        private void CleanupSocket()
        {
            if (_socket == null) return;
            _socket.Dispose();
            _socket = null;
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private class McwssEventStructure
        {
            public McwssEventHeaders header { get; set; } = new McwssEventHeaders();
            public McwssEventBody body { get; set; } = new McwssEventBody();
        }

        private class McwssEventHeaders
        {
            public string requestId { get; set; } = string.Empty;
            public string eventName { get; set; } = string.Empty;
            public string messagePurpose { get; set; } = "subscribe";
            public int version { get; set; } = 1;
        }

        private class McwssEventBody
        {
            public string eventName { get; set; } = string.Empty;
        }

        private class McwssPlayerMessageStructure
        {
            public McwssEventHeaders header { get; set; } = new McwssEventHeaders();
            public McwssPlayerMessageBody body { get; set; } = new McwssPlayerMessageBody();
        }

        private class McwssPlayerMessageBody
        {
            public string message { get; set; } = string.Empty;
            public string receiver { get; set; } = string.Empty;
            public string sender { get; set; } = string.Empty;
            public string type { get; set; } = string.Empty;
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