using System;
using System.IO;

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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Token.ToByteArray());
        }

        public override void Deserialize(BinaryReader reader)
        {
            Token = new Guid(reader.ReadBytes(16));
        }
    }
}