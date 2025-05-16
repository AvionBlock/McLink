using System;
namespace MCLink.Packets
{
    public class Update : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Update;
        
        public Guid Token { get; set; }
        public object[] Packets { get; set; }
        
        public Update(Guid token = new Guid(), params object[]? packets)
        {
            Token = token;
            Packets = packets ?? Array.Empty<object>();
        }
    }
}