using System.IO;

namespace MCLink
{
    public abstract class McLinkPacket : IMcLinkSerializable
    {
        public virtual PacketType PacketType { get; } = PacketType.Unknown;
        
        public abstract void Serialize(BinaryWriter writer);
        
        public abstract void Deserialize(BinaryReader reader);
    }
}