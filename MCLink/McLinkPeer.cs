using System;
using System.Collections.Concurrent;

namespace MCLink
{
    public abstract class McLinkPeer
    {
        public Guid Token { get; } = Guid.NewGuid();
        
        internal ConcurrentQueue<byte[]> PacketQueue = new ConcurrentQueue<byte[]>();
    }
}