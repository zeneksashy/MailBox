using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Runtime.Serialization;
using MimeKit;
namespace MailBox
{
    [DataContract]
    public class Client
    {
        public string Name { get; private set; }
        public string setName()
        {
            if (!String.IsNullOrEmpty(Email))
            {
                var str = Email.Split('@')[0];
                if (str.Contains('.'))
                    str.Replace('.', '_');
                Name = str;
                return Name;
            }
            return "Empty_Email";
        }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public List<MimeMessage> mails = new List<MimeMessage>();
        private static readonly object locker = new object();
        private Client() { }
        private static Client client;
        public static Client GetInstance()
        {
           lock(locker)
            {
                if (client == null)
                    client = new Client();
                return client;
            }
        }
    }
}
