using System;
using System.IO;

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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Data.Length);
            if(Data.Length > 0)
                writer.Write(Data);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if(length > 0)
                Data = reader.ReadBytes(length);
        }
    }
}