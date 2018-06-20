﻿using System;
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
using System.Net.Mail;
using System.Threading;
using System.Xml;
using System.IO;
using MimeKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MailBox.Properties;
//TODO
//Show Attachments
// Reply to
// Send msg
//Filtering -- need testing
//Sorting  -- done
//Nicer look
//Logout -- done 
//Changing hosts -- done
//imap idle
//inbox add ?

namespace MailBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Imapfeatures features;
        string path;
        Client client = Client.GetInstance();
        List<MimeMessage> msg = new List<MimeMessage>();
        ImapClient imap;
        List<string> tempdirs = new List<string>();
        IMailFolder inbox;
        ImapClient idleclient;
        public MainWindow()
        {
            InitializeComponent();
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path +=@"\"+client.setName()+@"\mails";
            imap = new ImapClient();
            idleclient= new ImapClient();
            idleclient.Connect(client.Host, client.Port, true);
            idleclient.Authenticate(client.Email, client.Password);
            imap.Connect(client.Host, client.Port, true);
            imap.Authenticate(client.Email, client.Password);         
            inbox = imap.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            imap.IdleAsync(new CancellationToken());
        }
        private void LoadMessages()
        {
            foreach (var item in Fetch(inbox))
            {
                client.mails.Add(item);
                msg.Add(item);
            }
           
            ChangeVisibilities();
        }
        private void ChangeVisibilities()
        {
            features = new Imapfeatures(msg);
            msg = features.SortBy(SortFilters.Date, Order.DSC);
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
            progress_label.Dispatcher.Invoke(() => progress_label.Visibility = Visibility.Hidden);
            bar.Dispatcher.Invoke(() => bar.Visibility = Visibility.Hidden);
            browser.Dispatcher.Invoke(() => browser.Visibility = Visibility.Visible);
        }
        private void LoadMessages(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                msg.Add(MimeMessage.Load(file));
            }
            ChangeVisibilities();
        }
        IEnumerable<MimeMessage> Fetch(IMailFolder inbox)
        {    
            for (int i = 0; i <inbox.Count; i++)
            {           
                yield return inbox.GetMessage(i);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var previous = App.Current.MainWindow;
            if(previous!=this)
            {
                App.Current.MainWindow = this;
                previous.Close();
            }       
        }
        public void OpenInBrowser(int uid)
        {
            var message = msg[uid-1];
            StringBuilder sb = new StringBuilder();
            var from = getMailbox(message.From.Mailboxes);
            var to = getMailbox(message.To.Mailboxes);
            var date = message.Date;
            var subject = message.Subject;
            foreach (var adr in from)
            {
                sb.Append("OD: ").Append(adr).Append(" ");
            }
            foreach (var adr in to)
            {
                sb.Append("DO: ").Append(adr).Append(" ");
            }
            sb.AppendLine().Append("Temat: ").Append(subject).Append(" Data: ").Append(date);
            var tmp = System.IO.Path.Combine(path, "msg" + uid);
            var htmlpreview = new HtmlPreviewVisitor(tmp);
            Directory.CreateDirectory(tmp);
            tempdirs.Add(tmp);
            message.Accept(htmlpreview);
            text.Text = sb.ToString();
            browser.NavigateToString(htmlpreview.HtmlBody);
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if(!Settings.Default.isSaved)
            Task.Run(()=> LoadMessages());
            else
            {
                Task.Run(() => LoadMessages(path));
            }
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imap.Disconnect(true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            int i = 1;
            var sb = new StringBuilder(path);
            foreach (var item in msg)
            {
                sb.Append("\\msg").Append(i).Append(".eml");
                item.WriteTo(sb.ToString());
                i++;
                sb.Clear();
                sb.Append(path);
            }
            Settings.Default.isSaved = true;
            Settings.Default.Save();
            DeleteTemps();
        }
        private void DeleteTemps()
        {
            foreach (var dir in tempdirs)
            {
                Directory.Delete(dir);
            }
        }
        public void FetchIdle()
        {     
             inbox = imap.Inbox;
             inbox.Open(FolderAccess.ReadOnly);
            int count = msg.Count;
            for (int i = count; i < inbox.Count; i++)
            {
                msg.Add(inbox.GetMessage(i));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            imap.Disconnect(true);
            new LoginForm().Show();
            Settings.Default.host = String.Empty;
            Settings.Default.mail = String.Empty;
            Settings.Default.pass = String.Empty;
            Settings.Default.isSaved = false;
            Settings.Default.isAuthenticated = false;
            Settings.Default.Save();
            this.Hide();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            var parent = menu.Parent as MenuItem;
            var orders = Enum.GetNames(typeof(Order));
            var filters = Enum.GetNames(typeof(SortFilters));
            Order ord = Order.DSC;
            SortFilters sort = SortFilters.Date;
            foreach (var order in orders)
            {
                if((string)menu.Header == order)
                {
                    Enum.TryParse(order, out ord);
                    break;
                }
            }
            foreach (var filt in filters)
            {
                if ((string)parent.Header == filt)
                {
                    Enum.TryParse(filt, out sort);
                    break;
                }
            }
             msg= features.SortBy(sort, ord);
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }
    }
    
}
