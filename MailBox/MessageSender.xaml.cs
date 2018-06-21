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
using Microsoft.Win32;
using System.Net.Mail;
using System.Net;

namespace MailBox
{
    /// <summary>
    /// Interaction logic for MessageSender.xaml
    /// </summary>
    public partial class MessageSender : UserControl
    {
        OpenFileDialog fileDialog;
        int items=0;
        double top=370;
        double left=20;
        Client client = Client.GetInstance();
        HashSet<string> attachments = new HashSet<string>();
        public MessageSender()
        {
            InitializeComponent();
            fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            fileDialog.Filter= "All files (*.*)|*.*";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            box.Clear();
            box.GotFocus -= TextBox_GotFocus;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if(fileDialog.ShowDialog()==true)
           {
                attachments.Add(fileDialog.FileName);
                RenderNew(fileDialog.FileName);
           }
          
        }
        private bool CheckChanges()
        {
            if(items!= attachments.Count)
            {
                items = attachments.Count;
                return true;
            }
            return false;
        }
        private void RenderNew(string fileName)
        {
            fileName= fileName.Split('\\').Last();
            var border = new Border() { BorderBrush = Brushes.DimGray,BorderThickness=new Thickness(1,1,1,1),CornerRadius = new CornerRadius(0,8,0,8), Width=285};
            var textblock = new TextBlock() { Text = fileName, Margin = new Thickness(5, 0, 1, 2) };
            border.Child = textblock;
            Canvas.SetTop(border, top);
            Canvas.SetLeft(border, left);
            top -= 25;
            canvas.Children.Add(border);
        }
        private MailMessage CreateMessage()
        {
            MailMessage ms = new MailMessage();
            ms.From = new MailAddress(client.Email);
            if(attachments.Count>0)
            foreach (var item in attachments)
            {
                ms.Attachments.Add(new System.Net.Mail.Attachment(item));
            }
            //if(UDW.Text!="Bcc" || UDW.Text != "")
            //{
            //    var bcc = Splitted(UDW.Text);
            //    foreach (var item in bcc)
            //    {
            //        ms.Bcc.Add(new MailAddress(item));
            //    }
            //}
           
            var to = Splitted(To.Text);
            foreach (var item in to)
            {
                ms.To.Add(new MailAddress(item));
            }
            //if (DW.Text != "Bc" || DW.Text != "")
            //{
            //    var cc = Splitted(DW.Text);
            //    foreach (var item in cc)
            //    {
            //        ms.CC.Add(new MailAddress(item));
            //    }
            //}
         
            ms.Body = Message.Text;
            return ms;

        }
        private string[] Splitted(string text)
        {
            return text.Split(';');
        }
        private void Send_Button(object sender, RoutedEventArgs e)
        {
           //  Task.Run(()=>Send());
            Send();
        }
        private void Send()
        {
            var host = getSmtpHost();
            SmtpClient smtpClient = new SmtpClient(host, 465) { UseDefaultCredentials = false, Credentials = new NetworkCredential(client.Email, client.Password), EnableSsl = true, Timeout = 100000 };
            var msg = CreateMessage();
            smtpClient.Send(msg);
        }
        private string getSmtpHost()
        {
           
            var splittedhost = client.Host.Split('.');
            string smtphost = String.Empty;
            for (int i = 1; i < splittedhost.Length; i++)
            {
                smtphost += splittedhost[i] + ".";
            }
            var host = "smtp." + smtphost;
            host.Remove(host.Length - 1);
            return host;
        }

    }
}
