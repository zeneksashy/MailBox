using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System.IO;
using MailBox.Properties;
namespace MailBox
{
    public class Messages
    {
        List<MimeMessage> seen = new List<MimeMessage>();
        List<MimeMessage> notseen = new List<MimeMessage>();
        List<MimeMessage> unSorted = new List<MimeMessage>();
        List<MimeMessage> original = new List<MimeMessage>();
        List<MimeMessage> msg = new List<MimeMessage>();
        ImapClient imap;
        MailList mailList;
        IMailFolder inbox;
        string path;

        public Messages(ImapClient imap,IMailFolder inbox,string path, MailList mailList)
        {
            this.imap = imap;
            this.path = path;
            this.inbox = inbox;
            this.mailList = mailList;
        }
        public bool CheckIfRead(MimeMessage msg)
        {
            return notseen.Any(m => m.MessageId == msg.MessageId);
        }
        public void SaveToFile()
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
        public void RemoveFromServer(int uid,List<MimeMessage> messages)
        {
            var i = unSorted.IndexOf(messages.ElementAt(uid));
            imap.Inbox.AddFlags(i, MessageFlags.Deleted, false);
            imap.Inbox.Expunge();
        }
        public void RemoveFromPc(int uid)
        {
            if (Directory.Exists(path + "\\msg" + uid + ".eml"))
                File.Delete(path + "\\msg" + uid + ".eml");
        }
        public void RemoveFromLists(int uid)
        {
            var i = unSorted.IndexOf(msg.ElementAt(uid));
            var mess = unSorted.ElementAt(uid);
            unSorted.RemoveAt(i);
            if (seen.Any(m => m.MessageId == mess.MessageId))
                seen.Remove(mess);
            else
                notseen.Remove(mess);
        }
        public void MessageShown(int uid)
        {
            var mess = msg.ElementAt(msg.Count - uid);
            var i = unSorted.IndexOf(mess);
            
            if (!seen.Any(m => m.MessageId == mess.MessageId))
            {
                seen.Add(mess);
                notseen.Remove(notseen.Single(foo => foo.MessageId == mess.MessageId));
                inbox.AddFlags(i, MessageFlags.Seen, true);
                inbox.Expunge();
                mailList.MarkAsRead(uid - 1);
            }
        }
        public List<MimeMessage> LoadMessages()
        {
            foreach (var item in Fetch(inbox))
            {
                unSorted.Add(item);
            }
            foreach (var uid in inbox.Search(SearchQuery.NotSeen))
            {
                var message = inbox.GetMessage(uid);
                notseen.Add(message);
            }
            foreach (var uid in inbox.Search(SearchQuery.Seen))
            {
                var message = inbox.GetMessage(uid);
                seen.Add(message);
            }
            msg = unSorted;
            return new List<MimeMessage>(msg);
        }
        public List<MimeMessage> LoadMessages(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                unSorted.Add(MimeMessage.Load(file));
            }
            if (Check())
                LoadMessages(unSorted.Count);
            msg = unSorted;
            return new List<MimeMessage>(msg);
        }
        public List<MimeMessage> LoadMessages(int index)
        {
            foreach (var item in Fetch(inbox, index))
            {
                unSorted.Add(item);
            }
            msg = unSorted;
            return new List<MimeMessage>(msg);
        }
        IEnumerable<MimeMessage> Fetch(IMailFolder inbox, int startindex)
        {
            for (int i = startindex; i < inbox.Count; i++)
            {
                yield return inbox.GetMessage(i);
            }
        }
        IEnumerable<MimeMessage> Fetch(IMailFolder inbox)
        {
            for (int i = 0; i < inbox.Count; i++)
            {
                yield return inbox.GetMessage(i);
            }
        }
   
        private bool Check() => unSorted.Count < inbox.Count;
        public List<MimeMessage> AddToList(MimeMessage message)
        {
            unSorted.Add(message);
            msg = unSorted;
            return new List<MimeMessage>(msg);
        }
    }
}
