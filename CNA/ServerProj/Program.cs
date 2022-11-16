using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;

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

        ConcurrentDictionary<int, ConnectedClient> m_Clients;

        public Server(string ipAdress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAdress);
            m_TcpListener = new TcpListener(ip, port);
        }

        public void Start()
        {
            m_Clients = new ConcurrentDictionary<int, ConnectedClient>();
            int clientIndex = 0, connectionLimit = 4;
            m_TcpListener.Start();
            while (true) 
            { 
                
            }
            //Socket socket = m_TcpListener.AcceptSocket();
            
        }

        public void Stop()
        {
            m_TcpListener.Stop();
        }

        //private void ClientMethod(Socket socket)
        //{
        //    string receivedMessage = "";
        //    NetworkStream stream = new NetworkStream(socket,true);

        //    StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
        //    StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);

        //    streamWriter.WriteLine("You Have connected to the STREAM");
        //    streamWriter.Flush();

        //    while ((receivedMessage = streamReader.ReadLine()) != null)
        //    {
        //        receivedMessage = GetReturnMessage(receivedMessage);
        //        streamWriter.WriteLine(receivedMessage);
        //        streamWriter.Flush();
        //        if (receivedMessage == "end") 
        //        {
        //            break;
        //        }
        //    }

        //    socket.Close();
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
        StreamReader m_Reader;
        StreamWriter m_Writer;
        object m_ReadLock;
        object m_WriteLock;

        public ConnectedClient(Socket socket) 
        { 
            m_WriteLock = new object();
            m_ReadLock = new object();

            m_socket = socket;

            m_Stream = new NetworkStream(socket, true);

            m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
            m_Writer = new StreamWriter(m_Stream, Encoding.UTF8);
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

        public void Send(string message) 
        {
            lock (m_WriteLock) 
            {
                m_Writer.WriteLine(message);
                m_Writer.Flush();
            }
        }
    }
}
