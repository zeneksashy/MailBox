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
using MimeKit;
namespace MailBox
{
    /// <summary>
    /// Interaction logic for MailList.xaml
    /// </summary>
    public partial class MailList : UserControl
    {
        public MailList()
        {
            InitializeComponent();
        }
        public void ShowMessageList(List<MimeMessage> messages)
        {
            panel.Children.Clear();
            int i = 1;
            foreach (var msg in messages)
            {
                StringBuilder sb = new StringBuilder();
                var to = getMailbox(msg.To.Mailboxes);
                var from = getMailbox(msg.From.Mailboxes);
                var subject = msg.Subject;
                var date = msg.Date.ToString("G");
                string body="empty message";
                if (!String.IsNullOrEmpty(msg.TextBody))
                    body = msg.TextBody;
                body = Splitted(body);
                foreach (var adr in from)
                {
                    sb.Append("OD: ").Append(adr).Append(" ");
                }
                sb.AppendLine();
                foreach (var adr in to)
                {
                    sb.Append("DO: ").Append(adr).Append(" ");
                }             
                sb.AppendLine().Append("Temat: ").Append(subject).AppendLine().Append("Data: ").Append(date).AppendLine().Append(body).AppendLine().Append("______________________");
                var txtblck = CreateTextBlock();
                txtblck.Uid = i.ToString();
                txtblck.Text = sb.ToString();
                sb.Clear();
                panel.Children.Add(txtblck);
                i++;
            }
        }
        private TextBlock CreateTextBlock()
        {
            var txtblck = new TextBlock();
            txtblck.MouseEnter += TextBlock_MouseEnter;
            txtblck.MouseLeave += TextBlock_MouseLeave;
            txtblck.PreviewMouseDown += TextBlock_Click;
            txtblck.Foreground = Brushes.Black;
            return txtblck;
        }
        private string Splitted(string str)
        {
            var split= str.Split();
            string body=String.Empty;
            if(split.Length>5)
                for (int i = 0; i < 5; i++)
                {
                    body += split[i] + " ";
                }
            else
            {
                for (int i = 0; i < split.Length; i++)
                {
                    body += split[i] + " ";
                }
            }
            return body;
        }
        private void TextBlock_Click(object sender, EventArgs arg)
        {
            var block = sender as TextBlock;
            int uid = int.Parse(block.Uid);
            MainWindow win = App.Current.MainWindow as MainWindow;
            win.OpenInBrowser(uid);

        }
        private void TextBlock_MouseLeave(object sender, MouseEventArgs arg)
        {
            var block = sender as TextBlock;
            block.Foreground = Brushes.Black;
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs arg)
        {
            var block = sender as TextBlock;
            block.Foreground = Brushes.Blue;
        }
        private List<string> getMailbox(IEnumerable<MailboxAddress> addresses)
        {
            var listofadrs = new List<string>();
            foreach (var addres in addresses)
            {
                listofadrs.Add(addres.Address);
            }
            return listofadrs;
        }
    }
}
