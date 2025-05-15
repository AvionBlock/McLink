namespace MCLink.Packets
{
    public class Login : McLinkPacket
    {
        public byte[]? Data { get; set; }

        public Login()
        {
            PacketType = PacketType.Login;
        }
    }
}