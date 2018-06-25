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
using System.Net.Mail;
using System.Threading;
using System.Xml;
using System.IO;
using MimeKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MailBox.Properties;
using System.Text.RegularExpressions;
//TODO
//Show Attachments --  almost done, saving left -- done
// Reply to -- done
// Send msg -- done
//Filtering -- need testing -- done
//Sorting  -- done
//Nicer look --1/10 done
//Logout -- done 
//Changing hosts -- done
//imap idle -- almost done, -- done
//inbox add ?
//message deleting -- almost done, need testing -- 
//message updating 
//Checking on startup if any new messages reciewed -- done
//bug to fix  -- 
namespace MailBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private fileds
        private List<string> _replyTo = new List<string>();
        private string _replySubject = "";
        Imapfeatures features;
        string path;
        Client client = Client.GetInstance();
        List<uint> seen = new List<uint>();
        List<uint> notSeen = new List<uint>();
        List<MimeMessage> unSorted = new List<MimeMessage>();
        List<MimeMessage> original = new List<MimeMessage>();
        List<MimeMessage> msg = new List<MimeMessage>();
        ImapClient imap;
        HashSet<string> tempdirs = new HashSet<string>();
        IMailFolder inbox;
        ImapIdle idle;

        #endregion

        /// <summary>
        /// The main constructor of a window,Initializes default path for mails, creates imap clients and connects with server
        ///
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += @"\" + client.setName() + @"\mails";
            imap = new ImapClient();
            imap.Connect(client.Host, client.Port, true);
            imap.Authenticate(client.Email, client.Password);
            //imap.Inbox.MessageFlagsChanged += Inbox_MessageFlagsChanged; powiadomienie o usunięciu wiadomości
            inbox = imap.Inbox;
            inbox.Open(FolderAccess.ReadWrite);
            idle = new ImapIdle(inbox.Count);
 
        }


        #region private methods
        // Powiadomienie o usunięciu wiadomości, niezbyt potrzebne
        //private void Inbox_MessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        //{
        //    if (e.Flags == MessageFlags.Deleted)
        //        MessageBox.Show("Message deleted");
        //} 
        private void SaveToFile()
        {
            imap.Disconnect(true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            int i = 1;
            var sb = new StringBuilder(path);
            foreach (var item in unSorted)
            {
                sb.Append("\\msg").Append(i).Append(".eml");
                item.WriteTo(sb.ToString());
                i++;
                sb.Clear();
                sb.Append(path);
            }
            Settings.Default.isSaved = true;
            Settings.Default.Save();
        }
        private void ChangeVisibilities()
        {
            ShowMessages();
            progress_label.Dispatcher.Invoke(() => progress_label.Visibility = Visibility.Hidden);
            bar.Dispatcher.Invoke(() => bar.Visibility = Visibility.Hidden);
        }
        private void ShowMessages()
        {
            features = new Imapfeatures(msg);
            original = msg;
            msg = features.SortBy(SortFilters.Date, Order.DSC);
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }
        #region event handlers
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveToFile();
        }
     
        private void Reply_btn_Click(object sender, RoutedEventArgs e)
        {
            new Send.SendWindow(String.Join(";", _replyTo), "Re: " + _replySubject).Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var previous = App.Current.MainWindow;
            if (previous != this)
            {
                App.Current.MainWindow = this;
                previous.Close();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!Settings.Default.isSaved)
                Task.Run(() => LoadMessages());
            else
            {
                Task.Run(() => LoadMessages(path));
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
                if ((string)menu.Header == order)
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
            msg = features.SortBy(sort, ord);
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }

        private void Date_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            string date = Date.Text;
            SearchFilters filter;
            Enum.TryParse((string)menu.Header, out filter);
            msg = features.FilterBy(MessageParts.Date, filter, date);
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }

        private void Sbj_Contains_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string searchfilter = (string)Subject_Contains.Header;
                var parent = Subject_Contains.Parent as MenuItem;
                string msgpart = parent.Header as string;
                SearchFilters filter;
                MessageParts part;
                Enum.TryParse(msgpart, out part);
                Enum.TryParse(searchfilter, out filter);
                msg = features.FilterBy(part, filter, Sbj_Contains.Text);
                mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
            }
        }

        private void Sbj_Lenght_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var txtbx = sender as TextBox;
                var parent = txtbx.Parent as MenuItem;
                parent = parent.Parent as MenuItem;
                string searchfilter = parent.Header as string;
                parent = parent.Parent as MenuItem;
                string msgpart = parent.Header as string;
                SearchFilters filter;
                MessageParts part;
                Enum.TryParse(msgpart, out part);
                Enum.TryParse(searchfilter, out filter);
                msg = features.FilterBy(part, filter, txtbx.Text);
                mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
            }
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var txtbx = sender as TextBox;
                string searchfilter = (string)From_Contains.Header;
                var parent = From_Contains.Parent as MenuItem;
                string msgpart = parent.Header as string;
                SearchFilters filter;
                MessageParts part;
                Enum.TryParse(msgpart, out part);
                Enum.TryParse(searchfilter, out filter);
                msg = features.FilterBy(part, filter, txtbx.Text);
                mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
            }
        }
        private void Bdy_Contains_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                string searchfilter = (string)Body_Contains.Header;
                var parent = Body_Contains.Parent as MenuItem;
                string msgpart = parent.Header as string;
                SearchFilters filter;
                MessageParts part;
                Enum.TryParse(msgpart, out part);
                Enum.TryParse(searchfilter, out filter);
                msg = features.FilterBy(part, filter, Bdy_Contains.Text);
                mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
            }
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            msg = original;
            features.ResetFilters();
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DeleteTemps();
            idle.Finish();
        }

        private void HasAttachments_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            msg = features.FilterBy(MessageParts.Body, SearchFilters.HasAttachments, "");
            mails.Dispatcher.Invoke(() => mails.ShowMessageList(msg));
        }

        private void UserCtrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var userctrl = sender as Attachment;
            var attachment = userctrl.Attach;
            int uid = int.Parse(userctrl.Uid);
            var tmpdir = System.IO.Path.Combine(path, "attach" + uid);//powinno byc w folderze z numerem wiadomosci 

            if (!Directory.Exists(tmpdir))
                Directory.CreateDirectory(tmpdir);
            var attachmentpath = System.IO.Path.Combine(path, "attach" + uid) + "\\" + attachment.ContentDisposition.FileName;
            using (var stream = File.Create(attachmentpath))
                if (attachment is MessagePart)
                {
                    var part = (MessagePart)attachment;

                    part.Message.WriteTo(stream);
                }
                else
                {
                    var part = (MimePart)attachment;

                    part.Content.DecodeTo(stream);
                }
            System.Diagnostics.Process.Start(attachmentpath);
        }

        private void NewMessageButton_Click(object sender, RoutedEventArgs e)
        {
            new Send.SendWindow().Show();
        }

        private void RemoveFromServer(int uid)
        {
            var i = unSorted.IndexOf(msg.ElementAt(uid));
            imap.Inbox.AddFlags(i, MessageFlags.Deleted, false);
            imap.Inbox.Expunge();
        }
        private void RemoveFromPc(int uid)
        {
            if (Directory.Exists(path + "\\msg" + uid + ".eml"))
                File.Delete(path + "\\msg" + uid + ".eml");
        }

        #endregion

        #region imap methods
        /// <summary>
        /// fetches all the messages from inbox and add them to msg list
        /// </summary>
        private void LoadMessages()
        {
            foreach (var item in Fetch(inbox))
            {
                client.mails.Add(item);
                unSorted.Add(item);
                msg.Add(item);
            }
            foreach (var uid in inbox.Search(SearchQuery.NotSeen))
            {
                notSeen.Add(uid.Id);             
               // var message = inbox.GetMessage(uid);
                //unSorted.Add(message);
            }
            foreach (var uid in inbox.Search(SearchQuery.Seen))
            {
                seen.Add(uid.Id);
            //    var message = inbox.GetMessage(uid);
            //    unSorted.Add(message);
            }
            ChangeVisibilities();
        }
        private void LoadMessages(int index)
        {
            foreach (var item in Fetch(inbox,index))
            {
                client.mails.Add(item);
                msg.Add(item);
            }
        }
        /// <summary>
        /// Load mails from path and adds them to msg list
        /// </summary>
        /// <param name="path"> path of a mails </param>
        private void LoadMessages(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                unSorted.Add(MimeMessage.Load(file));

            }
            if (Check())
                LoadMessages(unSorted.Count);
            msg = unSorted;
            ChangeVisibilities();
        }
        private bool Check() => unSorted.Count < inbox.Count;

        /// <summary>
        /// fetches all messages in inbox
        /// </summary>
        /// <param name="inbox"> inbox</param>
        /// <returns> retruns abstract list of mails </returns>
        IEnumerable<MimeMessage> Fetch(IMailFolder inbox)
        {
            for (int i = 0; i < inbox.Count; i++)
            {
               // uids.Add(i);
                yield return inbox.GetMessage(i);
            }
        }
        IEnumerable<MimeMessage> Fetch(IMailFolder inbox,int startindex)
        {
            for (int i = startindex; i < inbox.Count; i++)
            {
               // uids.Add(i);
                yield return inbox.GetMessage(i);
            }
        }
        /// <summary>
        /// Used to get mailboxes from "To" and "From" list and return them as string
        /// </summary>
        /// <param name="addresses"> list of mailboxes</param>
        /// <returns>list of mialboxes as string</returns>
        private List<string> getMailbox(IEnumerable<MailboxAddress> addresses)
        {
            var listofadrs = new List<string>();
            foreach (var addres in addresses)
            {
                listofadrs.Add(addres.Address);
            }
            return listofadrs;
        }
        #endregion

        /// <summary>
        /// Deletes temporary files from appdata directory
        /// </summary>
        private void DeleteTemps()
        {
            foreach (var dir in tempdirs)
            {
                Directory.Delete(dir, true);
            }
        }
        #endregion
        #region public methods
        public void AddToList(MimeMessage message)
        {
            unSorted.Add(message);
            msg.Add(message);
            ShowMessages();
        }
        /// <summary>
        /// Deletes email on a given uid on server side
        /// </summary>
        /// <param name="uid">uid of message to delete</param>
        /// 
        public void DeleteMessage(int uid)
        {
            uid--;
            RemoveFromPc(uid);
            RemoveFromServer(uid);
            var i = unSorted.IndexOf(msg.ElementAt(uid));
            unSorted.RemoveAt(i);
           //unSorted.Remove(msg.ElementAt(uid));
           msg.RemoveAt(uid);
        }
        /// <summary>
        /// Shows a message in a browser
        /// </summary>
        /// <param name="uid">uid of message which we want to show</param>
        public void OpenInBrowser(int uid)
        {
            var message = msg[uid - 1];
            browserPanel.ChangeTarget(message, uid, path, tempdirs);
        }
        #endregion

        

      
    }


}