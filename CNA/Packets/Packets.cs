using System.Net.Sockets;

namespace Packets
{
    [Serializable]
    public class Packet
    {
        public enum PacketType
        {
            ChatMessage,
            PrivateMessage,
            CleintName
        }
        public PacketType packetType
        {
            get
            {
                return packetType;
            }
            protected set
            {
                packetType = value;
            }
        }
    }

    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string message { get; private set; }

        public ChatMessagePacket(string message)
        {
            this.message = message;
            this.packetType = PacketType.ChatMessage;
        }
    }

    //[Serializable]
    //class PrivateMessage : Packet
    //{
    //    public string message;
    //    public string Name;

    //    public PrivateMessage(string message, string Name)
    //    {
    //        this.message = message;
    //        this.Name = Name;
    //    }
    //}

    //[Serializable]
    //class ClientName : Packet
    //{
    //    public string Name;

    //    public ClientName(string Name)
    //    {
    //        this.Name = Name;
    //    }
    //}
}