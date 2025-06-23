using System;
using System.IO;
using MCLink.Utils;

namespace MCLink.Packets
{
    public class Logout : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Logout;
        public Guid Token { get; private set; }
        
        public Logout(Guid token = new Guid())
        {
            Token = token;
        }

        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Token.ToByteArray());
        }

        public override void Deserialize(NetDataReader reader)
        {
            Token = reader.GetGuid();
        }
    }
}