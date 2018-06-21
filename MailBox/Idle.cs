using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using System.Threading;
using MailKit.Net.Imap;

namespace MailBox
{
    class ImapIdle
    {
        ImapClient imapClient;
        ImapClient questClient;
        object locker = new object();
        Client client = Client.GetInstance();
        CancellationTokenSource done = new CancellationTokenSource() ;
        int count;
        public ImapIdle(int count)
        {
            questClient = new ImapClient();
            imapClient = new ImapClient();
            this.count = count;
            SetClient(questClient);
            SetClient(imapClient);
            imapClient.Inbox.CountChanged += Inbox_CountChanged;
            Task.Run(() => imapClient.Idle(done.Token));          
        }
        private  void SetClient(ImapClient iclient)
        {
            iclient.Disconnect(true);
            iclient.Connect(client.Host, client.Port, true);
            iclient.Authenticate(client.Email, client.Password);
            iclient.Inbox.Open(FolderAccess.ReadOnly);
        }

        private void Inbox_CountChanged(object sender, EventArgs e)
        {
            var folder = (ImapFolder)sender;
            SetClient(questClient);
            for (int i = count; i < folder.Count; i++)
            {
                var msg =  questClient.Inbox.GetMessage(i);
                //Exception -- Nie mozna pobrac z innego watku
                //******************************
                //MainWindow win;
                //win = App.Current.MainWindow.Dispatcher.Invoke(()=> win = App.Current.MainWindow as MainWindow);
                //win.AddToList(msg);
            }
            lock(locker)
            count = folder.Count;
        }
        public void Finish()
        {
            done.Cancel();
        }
    }
}
        
    
