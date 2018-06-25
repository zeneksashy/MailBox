using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit;
using System.IO;
using MailBox.Properties;
namespace MailBox
{
    class Message
    {
        public MimeMessage mimeMessage { get; private set; }
        public bool isSeen { get;  set; }
        public uint Uid { get; private set; }
        public Message(MimeMessage msg, bool Seen,uint uid)
        {
            mimeMessage = msg;
            isSeen = Seen;
            Uid = uid;
        }
     //   private void SaveToFile(string path)
        //{
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);
        //    var sb = new StringBuilder(path);
        //    sb.Append()
        //    mimeMessage.WriteTo(path)
        //    int i = 1;
           
        //    foreach (var item in msg)
        //    {
        //        sb.Append("\\msg").Append(i).Append(".eml");
        //        item.WriteTo(sb.ToString());
        //        i++;
        //        sb.Clear();
        //        sb.Append(path);
        //    }
        //    Settings.Default.isSaved = true;
        //    Settings.Default.Save();
        //}
    }
}
