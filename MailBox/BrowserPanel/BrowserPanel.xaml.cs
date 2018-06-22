using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MailBox.BrowserPanel
{
    /// <summary>
    /// Interaction logic for BrowserPanel.xaml
    /// </summary>
    public partial class BrowserPanel : UserControl
    {
        private MimeMessage _message;

        public BrowserPanel()
        {
            InitializeComponent();
        }

        //panel.Children.Clear();
        //    var message = msg[uid - 1];
        //    StringBuilder sb = new StringBuilder();
        //    var from = _replyTo = getMailbox(message.From.Mailboxes);
        //    var to = getMailbox(message.To.Mailboxes);
        //    var date = message.Date;
        //    var subject = _replySubject = message.Subject;
        //    foreach (var adr in from)
        //    {
        //        sb.Append("OD: ").Append(adr).Append(" ");
        //    }
        //    foreach (var adr in to)
        //    {
        //        sb.Append("DO: ").Append(adr).Append(" ");
        //    }
        //    sb.AppendLine().Append("Temat: ").Append(subject).Append(" Data: ").Append(date);
        //    var tmp = System.IO.Path.Combine(path, "msg" + uid);
        //    var htmlpreview = new HtmlPreviewVisitor(tmp);
        //    Directory.CreateDirectory(tmp);
        //    tempdirs.Add(tmp);
        //    message.Accept(htmlpreview);
        //    text.Text = sb.ToString();
        //    ShowAttachments(message);
        //    browser.NavigateToString(htmlpreview.HtmlBody);
        //    reply_btn.Visibility = Visibility.Visible;

        public void ChangeTarget(MimeMessage message, int uid, string path, HashSet<string> tempDirs)
        {
            _message = message;
            FillTextBlocks();
            FillBrowser(System.IO.Path.Combine(path, "msg" + uid), tempDirs);
        }

        private void FillTextBlocks()
        {
            infoStackPanel.DataContext = _message;
            fromTextBlock.Text = String.Join(" ", _message.From);
            toTextBlock.Text = String.Join(" ", _message.To);
        }

        private void FillBrowser(string fullPath, HashSet<string> tempDirs)
        {
            Directory.CreateDirectory(fullPath);
            tempDirs.Add(fullPath);
            HtmlPreviewVisitor htmlPreview = new HtmlPreviewVisitor(fullPath);
            _message.Accept(htmlPreview);
            browserBrowser.NavigateToString(htmlPreview.HtmlBody);
        }


        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            new Send.SendWindow(String.Join(";",_message.From), "Re: " +_message.Subject).Show();
        }
    }
}
