using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit;
namespace MailBox
{
    class Message
    {
        public MimeMessage mimeMessage { get; private set; }
        public bool isRead { get;  set; }
        public Message(MimeMessage msg, bool Read)
        {
            mimeMessage = msg;
            isRead = Read;
            //foreach (var uid in folder.Search(SearchQuery.NotSeen))
            //{
            //    var message = folder.GetMessage(uid);
            //}
        }
    }
}
