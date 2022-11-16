using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ClientProj
{
    internal class Program
    {
        public static void Main()
        {
            Client client = new Client();
            if (client.Connect("127.0.0.1", 4444)) client.Run();
            else Console.WriteLine("Faild to connect to server");
        }
    }

    public class Client 
    {
        private TcpClient m_TcpClient;
        private NetworkStream m_Stream;
        private StreamWriter m_Writer;
        private StreamReader m_Reader;

        public int Port { get; set; }

        public Client() 
        {
            m_TcpClient = new TcpClient();
            
        }

        public bool Connect(string ipAddress, int port) 
        {
            try
            {
                m_TcpClient.Connect("127.0.0.1", 4444);
                m_Stream = m_TcpClient.GetStream();
                m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
                m_Writer = new StreamWriter(m_Stream, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Run() 
        {
            string userinput = null;
            ProccessServerResponse();
            while ((userinput = Console.ReadLine()) != null) 
            {
                m_Writer.WriteLine(userinput);
                m_Writer.Flush();
                ProccessServerResponse();
                if (userinput == "exit") 
                {
                    break;
                }
            }
            m_TcpClient.Close();

        }

        private void ProccessServerResponse() 
        {
            Console.WriteLine("server says: " + m_Reader.ReadLine() + "\n");
        }
    }
}
