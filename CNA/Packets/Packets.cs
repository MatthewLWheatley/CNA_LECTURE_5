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
            ClientName,
            Empty,
            ClientList
        }
        public PacketType packetType { get; internal set; }
    }

    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string message { get; set; }

        public ChatMessagePacket(string message)
        {
            this.message = message;
            this.packetType = PacketType.ChatMessage;
        }
    }

    public class EmptyPacket : Packet 
    {
        public EmptyPacket() 
        {
            this.packetType = PacketType.Empty;
        }
    }

    [Serializable]
    public class PrivateMessagePacket : Packet
    {
        public string message;
        public string Name;

        public PrivateMessagePacket(string message, string Name)
        {
            this.message = message;
            this.Name = Name; 
            this.packetType = PacketType.PrivateMessage;
        }
    }

    [Serializable]
    public class ClientNamePacket : Packet
    {
        public string Name { get; set; }

        public ClientNamePacket(string Name)
        {
            this.Name = Name;
            this.packetType = PacketType.ClientName;
        }
    }

    [Serializable]
    public class ClientListPacket : Packet
    {
        public string[] Names { get; set; }

        public ClientListPacket(string[] Name)
        {
            this.Names = Name;
            this.packetType = PacketType.ClientList;
        }
    }
}