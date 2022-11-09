using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

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

        public Server(string ipAdress, int port)
        {
            IPAddress ip = IPAddress.Parse(ipAdress);
            m_TcpListener = new TcpListener(ip, port);
        }

        public void Start()
        {
            m_TcpListener.Start();
            Socket socket = m_TcpListener.AcceptSocket();
            ClientMethod(socket);
        }

        public void Stop()
        {
            m_TcpListener.Stop();
        }

        private void ClientMethod(Socket socket)
        {
            string receivedMessage = "";
            NetworkStream stream = new NetworkStream(socket,true);

            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);

            streamWriter.WriteLine("You Have connected to the STREAM");
            streamWriter.Flush();

            while ((receivedMessage = streamReader.ReadLine()) != null)
            {
                GetReturnMessage(receivedMessage);
                streamWriter.WriteLine(receivedMessage);
                streamWriter.Flush();
                if (receivedMessage == "end") 
                {
                    break;
                }
                socket.Close();
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
}
