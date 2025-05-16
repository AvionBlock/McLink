namespace MCLink.Packets
{
    public class Login : McLinkPacket
    {
        public override PacketType PacketType => PacketType.Login;
        public object? Data { get; set; }
    }
}