using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Packets;

namespace ServerProj
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 4444);
            server.Start();
            server.Stop();
        }
    }

    public class Server
    {
        private static TcpListener m_TcpListener;

        private ConcurrentDictionary<int, ConnectedClient> m_Clients;

        public Server(string ipAdress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAdress);
            m_TcpListener = new TcpListener(ip, port);
        }

        public void Start()
        {
            m_Clients = new ConcurrentDictionary<int, ConnectedClient>();
            int clientIndex = 0;
            m_TcpListener.Start();
            while (true)
            {
               
                // Accept incoming connections
                Socket socket = m_TcpListener.AcceptSocket();

                // Create a new ConnectedClient instance
                ConnectedClient client = new ConnectedClient(socket);

                // Add the client to the clients dictionary
                m_Clients.TryAdd(clientIndex, client);

                // Start a new thread to handle the client's requests
                Thread thread = new Thread(() => ClientMethod(clientIndex));
                thread.Start();
                Thread.Sleep(500);
                // Increment the client index
                clientIndex++;
            }
            
        }
        
        public void Stop()
        {
            m_TcpListener.Stop();
        }

        private void ClientMethod(int index)
        {
            Console.WriteLine(index);
            Packet receivedPacket = null;

            m_Clients[index].SendMessage(new ChatMessagePacket("hello there can i help you"));

            while ((receivedPacket = m_Clients[index].Read()) != null)
            {
                // Use a switch statement to handle the different types of packets
                switch (receivedPacket.packetType)
                {  
                    case Packet.PacketType.ChatMessage:
                        ChatMessagePacket chatMessagePacket = (ChatMessagePacket)receivedPacket;

                        string receivedMessage = GetReturnMessage(chatMessagePacket.message);
                        if (receivedMessage[0] == '/')
                        {
                            PrivateMessage(receivedMessage);
                        }
                        else
                        {
                            // Broadcast the message to all connected clients
                            BroadcastMessage(receivedMessage, index);
                        }
                        break;
                    case Packet.PacketType.ClientName:
                        string[] ClientNames = new string[m_Clients.Count];
                        m_Clients[index].SetClientName(receivedPacket,ClientNames);
                        for (int i = 0; i < m_Clients.Count; i++)
                        {
                            ClientNames[i] = m_Clients[i].m_Name;
                        }
                        BroadcastNames(ClientNames);

                        break;
                    case Packet.PacketType.PrivateMessage:

                        break;
                    case Packet.PacketType.Empty:

                        break;
                    default:

                        break;
                }

            }

            //m_Clients[index].Close();
        }

        public void PrivateMessage(string message) 
        {
            string newMessage = message.Substring(3);
            ChatMessagePacket messsagePacket = new ChatMessagePacket(DateTime.Now + " [" + m_Clients[(int)message[1] - 1].m_Name + "]: " + message);
            m_Clients[(int)message[1]-1].SendMessage(messsagePacket);
        }
        public void BroadcastNames(string[] names)
        {
            foreach (var client in m_Clients.Values)
            {
                ClientListPacket messsagePacket = new ClientListPacket(names);
                client.SendMessage(messsagePacket);
            }
        }

        public void BroadcastMessage(string message,int index)
        {
            foreach (var client in m_Clients.Values)
            {
                ChatMessagePacket messsagePacket = new ChatMessagePacket(DateTime.Now + " [" + m_Clients[index].m_Name + "]: " + message);
                client.SendMessage(messsagePacket);
            }
        }



        private string GetReturnMessage(string code)
        {
            if (code == "hi")
            {
                code = "hello";
            }
            return code;
        }
    }

    public class ConnectedClient
    {
        Socket m_socket;
        NetworkStream m_Stream;
        BinaryReader m_Reader;
        BinaryWriter m_Writer;
        BinaryFormatter m_Formatter;
        object m_ReadLock;
        object m_WriteLock;
        public string m_Name { get; private set; }

        public ConnectedClient(Socket socket)
        {
            m_Name = " "; 


            m_WriteLock = new object();
            m_ReadLock = new object();

            m_socket = socket;

            m_Stream = new NetworkStream(socket, true);

            m_Reader = new BinaryReader(m_Stream, Encoding.UTF8);
            m_Writer = new BinaryWriter(m_Stream, Encoding.UTF8);
            m_Formatter = new BinaryFormatter();
        }

        public void Close()
        {
            m_Stream.Close();
            m_Reader.Close();
            m_Writer.Close();
            m_socket.Close();
        }

        public Packet Read()
        {
            lock (m_ReadLock)
            {
                try
                {
                    // Create a memory stream that contains the serialized data
                    int numberOfBytes = m_Reader.ReadInt16();

                    if (numberOfBytes > 0)
                    {
                        byte[] data = m_Reader.ReadBytes(numberOfBytes);
                        byte[] newData = new byte[data.Length];
                        Array.Copy(data, 2, newData, 0, newData.Length - 2);
                        Array.Copy(data, 0, newData, newData.Length - 2, 2);
                        Array.Copy(newData, 0, data, 0, newData.Length);

                        //for (int i = 0; i < numberOfBytes; i++)
                        //{
                        //    Console.Write(data[i] + " ");
                        //}

                        MemoryStream ms = new MemoryStream(data);
                        m_ReadLock = new object();
                        return (Packet)m_Formatter.Deserialize(ms) as Packet;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while reading packet: {ex.Message}");
                } 
                return new EmptyPacket();
            }
        }

        public void SendMessage(Packet packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                m_Formatter.Serialize(ms, packet);

                byte[] buffer = ms.GetBuffer();

                //for (int i = 0; i < buffer.Length; i++)
                //{
                //    Console.Write(buffer[i] + " ");
                //}

                m_Writer.Write(buffer.Length);
                m_Writer.Write(buffer);
                m_Writer.Flush();
            }
        }

        public void SetClientName(Packet receivedPacket, string[] ClientCount) 
        {
            ClientNamePacket NamePacket = (ClientNamePacket)receivedPacket;
            m_Name = NamePacket.Name;
        }
    }
}