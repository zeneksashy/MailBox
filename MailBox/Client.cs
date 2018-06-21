using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
namespace MailBox
{
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
        public string Email { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<MimeMessage> mails = new List<MimeMessage>();
        private static readonly object locker = new object();
        private Client() { Port = 993; }
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
