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
using System.Net.Mail;
using MailKit.Net.Imap;

namespace MailBox
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        bool hostchanged=false;
        Client client = Client.GetInstance();
        public LoginForm()
        {
            InitializeComponent();
            App.Current.MainWindow = this;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }
        private void Connect()
        {
            if (Check())
            {
                client.Email = mail_textbox.Text;
                client.Password = passwordbox.Password;
                Settings.Default.isAuthenticated = true;
                Settings.Default.host = client.Host;
                Settings.Default.mail = client.Email;
                Settings.Default.pass = client.Password;
                Settings.Default.Save();
                
                var main = new MainWindow();
                main.Show();
            }
        }
        private bool Check()
        {
            if (mail_textbox.Text == "" || passwordbox.Password == "")
            {
                MessageBox.Show("pola nie moga byc puste");
                return false;
            }
            string imap = String.Empty;
            if (!hostchanged)
            {
                imap = CheckHost(mail_textbox.Text);
                client.Host = imap;
            }    
            try
            {
                using (var imapclient = new ImapClient())
                {
                    imapclient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    imapclient.Connect(client.Host, client.Port, true);
                    imapclient.Authenticate(mail_textbox.Text, passwordbox.Password);
                }
                return true;
            }
            catch (MailKit.Security.AuthenticationException)
            {
                MessageBox.Show("Zły login lub hasło");
            }
            catch(System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show("Nie prawidłowy host lub port, ustaw manualnie");
                new HostCHange().Show();
                Task.Run(() => ListenChanges(imap));         
            }
            return false;
        }
        private void ListenChanges(string imap)
        {
            while (client.Host == imap)
            {
            }
            hostchanged = true;
            this.Dispatcher.Invoke(()=>Connect());
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Connect();
        }
        private string CheckHost(string mail)
        {
            MailAddress address = new MailAddress(mail);
            switch(address.Host)
            {
                case "gmail.com": return "imap." + address.Host; 
                case "o2.pl": return "poczta." + address.Host;
                case "interia.pl": return "poczta." + address.Host;
                case "wp.pl": return "imap." + address.Host;
                case "onet.pl": return "imap.poczta." + address.Host;
                case "outlook.com": return "imap-mail." + address.Host;
                default: return "imap." + address.Host;
            }
        }
    }
}
