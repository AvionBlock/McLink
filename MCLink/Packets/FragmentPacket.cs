using System;
using MCLink.Utils;

namespace MCLink.Packets
{
    public class FragmentPacket : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Fragment;
        public int FragmentId { get;  private set; }
        public bool End { get; private set; }
        public byte[] Data { get; private set; }

        public FragmentPacket(int fragmentId = 0, bool end = false, byte[]? data = null)
        {
            FragmentId = fragmentId;
            End = end;
            Data = data ?? Array.Empty<byte>();
        }

        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(FragmentId);
            writer.Put(End);
            writer.Put(Data.Length);
            if(Data.Length > 0)
                writer.Put(Data);
        }

        public override void Deserialize(NetDataReader reader)
        {
            FragmentId = reader.GetInt();
            End = reader.GetBool();
            var dataLength = reader.GetInt();
            if (dataLength == 0) return;
            Data = new byte[dataLength];
            reader.GetBytes(Data, dataLength);
        }
    }
}