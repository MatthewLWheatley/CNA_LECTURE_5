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

namespace CNA_LECTURE_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string temp = SendMessage(MessagerBox.Text,LocalName.Text);
            if (temp != "") 
            {
                ChatBox.Text += temp;
                MessagerBox.Text = "";
            }
            
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private string SendMessage(string message, string name) 
        {
            if (message == "" || name == "")
            {
                MessageBox.Show("Please Fill Out The Required Boxes.", "Straight Oopsies");
                return "";
            }
            else
            {

                return DateTime.Now + " [" + name + "]: " + message + "\n";
            }
        }
    }
}
