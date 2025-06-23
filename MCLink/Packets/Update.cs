using System;
using MCLink.Utils;

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
        
        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Token);
            if (Packets.Length <= 0)
                return;
            
            writer.Put(Packets.Length);
            foreach (var packet in Packets)
            {
                writer.Put(packet.Length);
                writer.Put(packet);
            }
        }

        public override void Deserialize(NetDataReader reader)
        {
            Token = reader.GetGuid();
            
            var packets = reader.GetInt();
            if (packets <= 0) return;
            var newPackets = new byte[packets][];

            for (var i = 0; i < packets; i++)
            {
                var packetLength = reader.GetInt();
                newPackets[i] = new byte[packetLength];
                reader.GetBytes(newPackets[i], packetLength);
            }
            
            Packets = newPackets;
        }
    }
}