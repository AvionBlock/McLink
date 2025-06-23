using MCLink.Utils;

namespace MCLink.Interfaces
{
    public interface IMcLinkSerializable
    {
        void Serialize(NetDataWriter writer);
        
        void Deserialize(NetDataReader reader);
    }
}