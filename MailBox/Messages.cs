using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Imap;

namespace MailBox
{
    class Messages
    {
        List<uint> seen = new List<uint>();
        List<uint> notSeen = new List<uint>();
        List<MimeMessage> unSorted = new List<MimeMessage>();
        List<MimeMessage> original = new List<MimeMessage>();
        List<MimeMessage> msg = new List<MimeMessage>();
        ImapClient imap;
        //TODO
        public Messages(ImapClient imap)
        {
            this.imap = imap;
        }
        private void SaveToFile()
        {

        }
        //todo
        private void ShowMessages() { }
        //TODO
        private void RemoveFromServer(int uid) { }
        private void RemoveFromPc(int uid) { }
        private void LoadMessages() { }
        private void LoadMessages(int index) { }
        private void LoadMessages(string path) { }
        //RObic czy nie ?
        public void AddToList(MimeMessage message) { }
        public void OpenInBrowser(int uid) { }
    }
}
