namespace MCLink
{
    public class McLinkPacket
    {
        public virtual PacketType PacketType { get; } = PacketType.Unknown;
    }
}