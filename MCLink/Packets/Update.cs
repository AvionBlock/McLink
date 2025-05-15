using System;

namespace MCLink.Packets
{
    public class Update : McLinkPacket
    {
        public Guid Token { get; set; }
        public byte[][] Packets { get; set; } = Array.Empty<byte[]>();
        
        public Update()
        {
            PacketType = PacketType.Update;
        }
    }
}