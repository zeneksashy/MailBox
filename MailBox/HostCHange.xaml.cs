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
using System.Windows.Shapes;
using MailBox.Properties;

namespace MailBox
{
    /// <summary>
    /// Interaction logic for HostCHange.xaml
    /// </summary>
    public partial class HostCHange : Window
    {
        public HostCHange()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cl = Client.GetInstance();
            cl.Host = HostBox.Text;
            cl.Port = Convert.ToInt32(PortBox.Text);
            Settings.Default.host = cl.Host;
            Settings.Default.port = cl.Port;
            Settings.Default.Save();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PortBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= PortBox_GotFocus;
        }
    }
}
