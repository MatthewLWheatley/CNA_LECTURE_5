using System.Net.Sockets;

namespace Packets
{
    enum PacketType
    {
        ChatMessage,
        PrivateMessage,
        CleintName
    }

    [Serializable]
    public class Packet
    {
        PacketType packetType
        {
            get
            {
                return packetType;
            }
            set
            {
                packetType = value;
            }
        }
    }

    [Serializable]
    class ChatMessagePacket : Packet
    {
        public string message;

        public ChatMessagePacket(string message)
        {
            this.message = message;
        }
    }
}