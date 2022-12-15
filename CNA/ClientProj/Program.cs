using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Packets;
using System.Runtime.Serialization.Formatters.Binary;

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

        private BinaryFormatter m_Formatter = new BinaryFormatter();
        private BinaryWriter m_Writer;
        private BinaryReader m_Reader;


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
                m_Reader = new BinaryReader(m_Stream, Encoding.UTF8);
                m_Writer = new BinaryWriter(m_Stream, Encoding.UTF8);
                m_Formatter = new BinaryFormatter();
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
            form = new MainWindow(this);
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

                    // Create a memory stream that contains the serialized data
                    int numberOfBytes = m_Reader.ReadInt16();
                    byte[] data = m_Reader.ReadBytes(numberOfBytes);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        // Deserialize the object
                        Packet obj = (Packet)m_Formatter.Deserialize(ms);

                        form.UpdateChatBox("" + "\n");
                    }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    break;
                }
            }

        }

        public void Send(String message)
        {
            lock (m_Writer)
            {
                MemoryStream ms = new MemoryStream();

                ChatMessagePacket chatmessage = new ChatMessagePacket(message);
                m_Formatter.Serialize(ms, chatmessage);
                byte[] bytes = ms.GetBuffer();
                Int32 length = bytes.Length;

                m_Writer.Write(length);
                m_Writer.Write(bytes);
                m_Writer.Flush();
            }
        }
    }
}
