using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

namespace MailBox.Send
{
    /// <summary>
    /// Interaction logic for SendWindow.xaml
    /// </summary>
    public partial class SendWindow : Window
    {
        private OpenFileDialog _openFileDialog;
        private Client _client;
        private HashSet<string> _attachments;

        public SendWindow()
        {
            Initialize();
        }

        public SendWindow(string to)
        {
            Initialize();
            toTextBox.Text = to;
        }

        private void Initialize()
        {
            InitializeComponent();

            _openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "All files (*.*)|*.*"
            };

            _client = Client.GetInstance();
            _attachments = new HashSet<string>();
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                _attachments.Add(_openFileDialog.FileName);
                attachments.Children.Add(GetNewAttachment(_openFileDialog));
            }
        }

        private Attachment GetNewAttachment(OpenFileDialog file)
        {
            Attachment attachment = new Attachment(file);
            attachment.MouseLeftButtonDown += Attachment_MouseLeftButtonDown;
            return attachment;
        }

        private void Attachment_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Attachment attachment = sender as Attachment;
            _attachments.Remove(attachment.FilePath);
            attachments.Children.Remove(attachment);
        }

        private SmtpClient GetSmtpClient()
        {
            return new SmtpClient(GetSmtpHostAddress(), 587)
            {
                Credentials = new NetworkCredential(_client.Email, _client.Password),
                EnableSsl = true,
            };
        }

        private string GetSmtpHostAddress()
        {

            string[] hostSplitted = _client.Host.Split('.');
            hostSplitted[0] = "smtp";
            return string.Join(".", hostSplitted);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(toTextBox.Text))
                MessageBox.Show("Receiver box cannot be empty");
            else
                SendMessage();
        }

        private MailMessage GetMailMessage()
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_client.Email);

            foreach (var item in _attachments)
                mailMessage.Attachments.Add(new System.Net.Mail.Attachment(item));

            if (!string.IsNullOrEmpty(ccTextBox.Text))
                mailMessage.CC.Add(ccTextBox.Text.Replace(';', ','));

            if (!string.IsNullOrEmpty(bccTextBox.Text))
                mailMessage.Bcc.Add(bccTextBox.Text.Replace(';', ','));

            if (!string.IsNullOrEmpty(toTextBox.Text))
                mailMessage.To.Add(toTextBox.Text.Replace(';', ','));


            mailMessage.Subject = subjectTextBox.Text;
            mailMessage.Body = bodyTextBox.Text;

            return mailMessage;
        }

        private async void SendMessage()
        {
            //Jakaś czarna magia z certyfikatem SSL
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate,
             X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

            SmtpClient smtpClient = GetSmtpClient();
            smtpClient.SendCompleted += SmtpClient_SendCompleted;
            await smtpClient.SendMailAsync(GetMailMessage());
        }

        private void SmtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("E-mail sent");

        }
    }
}

