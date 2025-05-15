using System.Collections.Generic;

namespace MCLink
{
    public abstract class McLinkServer
    {
        protected readonly List<McLinkPeer> _connectedPeers = new List<McLinkPeer>();
        protected readonly List<McLinkConnectionRequest> _connectionRequests = new List<McLinkConnectionRequest>();
        
        public IEnumerable<McLinkPeer> ConnectedPeers => _connectedPeers;
        public IEnumerable<McLinkConnectionRequest> ConnectionRequests => _connectionRequests;

        public abstract void Start(int port);

        public abstract void Stop();
    }
}