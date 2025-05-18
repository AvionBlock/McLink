using System;
using System.IO;

namespace MCLink.Packets
{
    public class Update : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Update;
        
        public Guid Token { get; private set; }
        public byte[][] Packets { get; private set; }
        
        public Update(Guid token = new Guid(), params byte[][]? packets)
        {
            Token = token;
            Packets = packets ?? Array.Empty<byte[]>();
        }
        
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Token.ToByteArray());
            if (Packets.Length <= 0)
                return;

            writer.Write(Packets.Length);
            foreach (var packet in Packets)
            {
                writer.Write(packet.Length);
                writer.Write(packet);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            Token = new Guid(reader.ReadBytes(16));
            
            var packets = reader.ReadInt32();
            if (packets <= 0) return;
            var newPackets = new byte[packets][];

            for (var i = 0; i < packets; i++)
            {
                var packetLength = reader.ReadInt32();
                newPackets[i] = reader.ReadBytes(packetLength);
            }
            
            Packets = newPackets;
        }
    }
}