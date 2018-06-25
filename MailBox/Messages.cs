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
        List<uint> seen = new List<uint>();
        List<uint> notSeen = new List<uint>();
        List<MimeMessage> unSorted = new List<MimeMessage>();
        List<MimeMessage> original = new List<MimeMessage>();
        List<MimeMessage> msg = new List<MimeMessage>();
        ImapClient imap;
        IMailFolder inbox;
        string path;
        //TODO

        public Messages(ImapClient imap,IMailFolder inbox,string path)
        {
            this.imap = imap;
            this.path = path;
            this.inbox = inbox;
        }
        public bool CheckIfRead(uint uid)
        {

            return seen.Contains(uid);
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
        //todo
        //public void ShowMessages() { }
        //TODO
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
            unSorted.RemoveAt(i);
            msg.RemoveAt(uid);
            if (seen.Contains((uint)i))
                seen.Remove((uint)i);
            else
                notSeen.Remove((uint)i);
        }
        public void MessageShown(uint uid)
        {
            if(!seen.Contains(uid))
            {
                seen.Add(uid);
                notSeen.Remove(uid);
            }
            //inbox.AddFlags(uid, MessageFlags.Seen, true);
        }
        public List<MimeMessage> LoadMessages()
        {
            foreach (var item in Fetch(inbox))
            {
                unSorted.Add(item);
            }
            foreach (var uid in inbox.Search(SearchQuery.NotSeen))
            {
                notSeen.Add(uid.Id);

            }
            foreach (var uid in inbox.Search(SearchQuery.Seen))
            {
                seen.Add(uid.Id);
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
                // uids.Add(i);
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
