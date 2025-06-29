using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using MCLink.Packets;
using MCLink.Utils;

namespace MCLink.Servers
{
    public abstract class McLinkServer
    {
        protected readonly NetDataReader Reader = new NetDataReader();
        protected readonly ConcurrentDictionary<IPEndPoint, McLinkPeer> ConnectedPeerList = new ConcurrentDictionary<IPEndPoint, McLinkPeer>();
        protected readonly ConcurrentDictionary<IPEndPoint, McLinkConnectionRequest> ConnectionRequestsList = new ConcurrentDictionary<IPEndPoint, McLinkConnectionRequest>();
        
        public IEnumerable<McLinkPeer> ConnectedPeers => ConnectedPeerList.Values;
        public IEnumerable<McLinkConnectionRequest> ConnectionRequests => ConnectionRequestsList.Values;

        public abstract void Start(int port);

        public abstract void Stop();
        
        protected void HandlePacket(NetDataReader reader, IPEndPoint endPoint)
        {
            var packetId = (PacketType)reader.GetByte();
            switch (packetId)
            {
                case PacketType.Fragment:
                    var fragmentPacket = new FragmentPacket();
                    fragmentPacket.Deserialize(reader);
                    HandleFragmentPacket(fragmentPacket, endPoint);
                    break;
                case PacketType.Login: //Do connection request setup.
                    break;
            }
        }

        private void HandleFragmentPacket(FragmentPacket fragmentPacket, IPEndPoint endPoint)
        {
            if (!ConnectedPeerList.TryGetValue(endPoint, out var peer)) return;
            if (!peer.FragmentedPackets.TryGetValue(fragmentPacket.FragmentId, out var writer))
            {
                writer = new NetDataWriter();
                peer.FragmentedPackets.TryAdd(fragmentPacket.FragmentId, writer);
            }
            
            writer.Put(fragmentPacket.Data);
        }
    }
}