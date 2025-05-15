using System;

namespace MCLink.Packets
{
    public class Logout : McLinkPacket
    {
        public Guid Token { get; set; }
        
        public Logout()
        {
            PacketType = PacketType.Logout;
        }
    }
}