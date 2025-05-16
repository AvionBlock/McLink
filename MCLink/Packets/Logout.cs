using System;

namespace MCLink.Packets
{
    public class Logout : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Logout;
        public Guid Token { get; set; }
        
        public Logout(Guid token = new Guid())
        {
            Token = token;
        }
    }
}