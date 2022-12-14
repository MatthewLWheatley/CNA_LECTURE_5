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
using Packet;

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
            string receivedMessage = "";

            m_Clients[index].SendMessage("You Have Connected To The STREAM");

            while ((receivedMessage = m_Clients[index].Read()) != null)
            {
                receivedMessage = GetReturnMessage(receivedMessage);
                BroadcastMessage(receivedMessage);
                //m_Clients[index].SendMessage(receivedMessage);

                if (receivedMessage == "end")
                {
                    break;
                }
            }

            m_Clients[index].Close();

        }

        public void BroadcastMessage(string message)
        {
            foreach (var client in m_Clients.Values)
            {
                client.SendMessage(message);
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

        public string Read()
        {
            lock (m_ReadLock)
            {
                return m_Reader.ReadLine();
            }
        }

        public void SendMessage(Packet )
        {
            if (message == "")
            {
                return;
            }

            m_Writer.WriteLine(message);
            m_Writer.Flush();
        }
    }
}