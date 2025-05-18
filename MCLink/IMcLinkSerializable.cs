using System.IO;

namespace MCLink
{
    public interface IMcLinkSerializable
    {
        void Serialize(BinaryWriter writer);
    }
}