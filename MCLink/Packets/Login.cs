using System;
using System.IO;
using MCLink.Utils;

namespace MCLink.Packets
{
    public abstract class Login : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Login;
        public byte[] Data { get; private set; }

        public Login(byte[]? data = null)
        {
            Data = data ?? Array.Empty<byte>();
        }

        public override void Serialize(NetDataWriter writer)
        {
            writer.Put(Data.Length);
            if(Data.Length > 0)
                writer.Put(Data);
        }

        public override void Deserialize(NetDataReader reader)
        {
            var length = reader.GetInt();
            if (length <= 0) return;
            Data = new byte[length];
            reader.GetBytes(Data, length);
        }
    }
}