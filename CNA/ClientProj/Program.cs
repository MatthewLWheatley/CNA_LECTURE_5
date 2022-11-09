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
            Console.WriteLine("hello world");
        }
    }

    public class Client 
    {
        private TcpClient m_TcpClient;
        private NetworkStream m_Stream;
        private StreamWriter m_Wirter;
        private StreamReader m_Reader;

        public Client() 
        { 
        
        }

        public bool Connect(string ipAddress, int pool) 
        {
            return false;
        }

        public void Run() 
        { 
        
        }

        private void ProccessServerResponse() 
        { 
        
        }
    }
}
