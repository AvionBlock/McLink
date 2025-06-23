using MCLink.Interfaces;
using MCLink.Utils;

namespace MCLink
{
    public abstract class McLinkPacket : IMcLinkSerializable
    {
        public virtual PacketType PacketType { get; } = PacketType.Unknown;
        
        public abstract void Serialize(NetDataWriter writer);
        
        public abstract void Deserialize(NetDataReader reader);
    }
}