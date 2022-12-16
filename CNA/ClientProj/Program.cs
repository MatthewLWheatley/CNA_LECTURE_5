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
                int numberOfBytes = -1;
                try
                {
                    // Create a memory stream that contains the serialized data
                    numberOfBytes = m_Reader.ReadInt16();

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

                        // Deserialize the object
                        Packet receivedPacket = (Packet)m_Formatter.Deserialize(ms);
                        switch (receivedPacket.packetType)
                        {
                            case Packet.PacketType.ChatMessage:
                                ChatMessagePacket chatMessagePacket = (ChatMessagePacket)receivedPacket;
                                form.UpdateChatBox(chatMessagePacket.message + "\n");
                                break;
                            case Packet.PacketType.ClientList:
                                ClientListPacket clientListPacket = (ClientListPacket)receivedPacket;

                                form.UpdateChatBox(clientListPacket.Names);
                                break;
                            default:

                                break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(numberOfBytes);
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
                byte[] data = ms.GetBuffer();


                //for (int i = 0; i < data.Length; i++)
                //{
                //    Console.Write(data[i] + " ");
                //}

                m_Writer.Write(data.Length);
                m_Writer.Write(data);
                m_Writer.Flush();
            }
        }

        public void SetNickName(String NickName)
        {
            lock (m_Writer)
            {
                MemoryStream ms = new MemoryStream();

                ClientNamePacket NickNamePacket = new ClientNamePacket(NickName);
                m_Formatter.Serialize(ms, NickNamePacket);
                byte[] data = ms.GetBuffer();

                //for (int i = 0; i < data.Length; i++)
                //{
                //    Console.Write(data[i] + " ");
                //}

                m_Writer.Write(data.Length);
                m_Writer.Write(data);
                m_Writer.Flush();
            }
        }
    }
}
