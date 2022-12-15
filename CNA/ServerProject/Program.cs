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

            m_Clients[index].SendMessage(new ChatMessagePacket("hello"));

            while ((receivedPacket = m_Clients[index].Read()) != null)
            {
                // Use a switch statement to handle the different types of packets
                switch (receivedPacket.packetType)
                {
                    case Packet.PacketType.ChatMessage:
                        ChatMessagePacket chatMessagePacket = (ChatMessagePacket)receivedPacket;

                        string receivedMessage = GetReturnMessage(chatMessagePacket.message);

                        // Broadcast the message to all connected clients
                        //BroadcastMessage(receivedMessage);

                        break;
                }
            }

            m_Clients[index].Close();
        }

        //public void BroadcastMessage(string message)
        //{
        //    foreach (var client in m_Clients.Values)
        //    {
        //        client.SendMessage(message);
        //    }
        //}

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

        public ConnectedClient(Socket socket)
        {
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
                    if (m_Reader.ReadInt32() != -1)
                    {
                        Int32 length = m_Reader.ReadInt32();
                        byte[] buffer = m_Reader.ReadBytes(length);

                        MemoryStream ms = new MemoryStream(buffer);
                        return (Packet)m_Formatter.Deserialize(ms) as ChatMessagePacket;
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while reading packet: {ex.Message}");
                }
                return null;

                int numberOfBytes;
                if ((numberOfBytes = m_Reader.ReadInt32()) != -1)
                {
                    byte[] buffer = m_Reader.ReadBytes(numberOfBytes);
                    MemoryStream ms = new MemoryStream();
                    ms.Write(buffer);

                }
                return m_Reader.ReadLine();
            }
        }

        public void SendMessage(Packet message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                m_Formatter.Serialize(ms, message);
                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
                byte[] buffer = ms.GetBuffer();
                m_Writer.Write(buffer.Length);
                m_Writer.Write(buffer);
                m_Writer.Flush();
            }
        }
    }
}