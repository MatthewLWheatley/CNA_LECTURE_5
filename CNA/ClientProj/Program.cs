using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace ClientProj
{
    internal class Program
    {
        [STAThread]
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
        public MainWindow form;

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
            MainWindow form = new MainWindow(this);
            Thread thread = new Thread(ProcessServerResponse);
            thread.Start();
            form.Dispatcher.Invoke(() => form.ShowDialog());
        }

        public void ProcessServerResponse()
        {
            while (m_TcpClient.Connected)
            {
                try
                {
                    string response = m_Reader.ReadLine();
                    form.UpdateChatBox("server says: " + response + "\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    break;
                }
            }
        }

        public void SendMessage(string message)
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
