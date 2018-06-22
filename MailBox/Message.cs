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
        public bool isSeen { get;  set; }
        public Message(MimeMessage msg, bool Seen)
        {
            mimeMessage = msg;
            isSeen = Seen;
        }
    }
}
