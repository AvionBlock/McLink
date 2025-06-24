using System;
using System.Collections.Concurrent;
using MCLink.Utils;

namespace MCLink
{
    public abstract class McLinkPeer
    {
        public Guid Token { get; } = Guid.NewGuid();
        internal readonly ConcurrentQueue<byte[]> OutgoingPacketQueue = new ConcurrentQueue<byte[]>();
        internal readonly ConcurrentQueue<byte[]> IncomingPacketQueue = new ConcurrentQueue<byte[]>();
        internal readonly ConcurrentDictionary<int, NetDataWriter> FragmentedPackets = new ConcurrentDictionary<int, NetDataWriter>();
    }
}