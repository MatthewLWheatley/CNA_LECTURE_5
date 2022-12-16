using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Packets;

namespace ClientProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client m_client;
        public bool nameSet = false;
        public MainWindow(Client client)
        {
            InitializeComponent();
            m_client = client;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string temp = SendMessage(MessagerBox.Text, LocalName.Text);

            if (temp != "")
            {
                m_client.Send(temp);
            }
        }

        public void UpdateChatBox(string message)
        {
            ChatBox.Dispatcher.Invoke(() =>
            {
                ChatBox.Text += message + Environment.NewLine; ChatBox.ScrollToEnd();
            });
        }

        public void UpdateChatBox(string[] Clients)
        {
            ClientList.Dispatcher.Invoke(() => ClientList.Clear());
            ClientList.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < Clients.Length; i++)
                {
                    ClientList.Text += Clients[i] + Environment.NewLine; ChatBox.ScrollToEnd(); 
                }
            });
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private string SendMessage(string message, string name)
        {
            if (message == "" || name == "" || nameSet == false)
            {
                MessageBox.Show("Please Fill Out The Required Boxes.", "Straight Oopsies");
                return ""; 
            }
            else
            {
                return /*DateTime.Now + " [" + name + "]: " +*/ message/* + "\n"*/;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetName(LocalName.Text);
        }

        private void SetName(string name) 
        {
            if (name == "")
            {
                MessageBox.Show("Please Fill Out The Required Boxes.", "Straight Oopsies");
            }
            else
            {
                nameSet = true;
                m_client.SetNickName(name);
            }
        }
    }
}
