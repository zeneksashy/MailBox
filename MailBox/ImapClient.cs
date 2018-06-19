using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MimeKit;

namespace MailBox
{
    //class ImapClient1
    //{
    //    static System.IO.StreamWriter sw = null;
    //    static System.Net.Sockets.TcpClient tcpc = null;
    //    static System.Net.Security.SslStream ssl = null;
    //    static string username, password;
    //    static string path;
    //    static int bytes = -1;
    //    static byte[] buffer;
    //    static StringBuilder sb = new StringBuilder();
    //    static byte[] dummy;
    //    bool Authed { get; set; }
    //    public ImapClient(string username, string pass, string host, int port=993)
    //    {
    //        Connect(username, pass, host, port);
    //    }
    //    void Connect(string username,string password, string host, int port=993)
    //    {
    //        path = Environment.CurrentDirectory + "\\emailresponse.txt";

    //        if (System.IO.File.Exists(path))
    //            System.IO.File.Delete(path);
    //        sw = new System.IO.StreamWriter(System.IO.File.Create(path));
    //        tcpc = new TcpClient(host,port);
    //        ssl = new System.Net.Security.SslStream(tcpc.GetStream());
    //        ssl.AuthenticateAsClient(host);
    //        receiveResponse("");
    //        string s = receiveResponse("$ LOGIN " + username + " " + password + "  \r\n");
    //        if (CheckMessage(s))
    //            Authed = false;
    //        else
    //            Authed = true;
    //       if(CheckMessage(GetListOfInboxes()))
    //        {

    //        }
    //        receiveResponse("$ SELECT INBOX\r\n");

    //        receiveResponse("$ STATUS INBOX (MESSAGES)\r\n");
    //        int number = 5;
            
    //        receiveResponse("$ FETCH " + number + " body[header]\r\n");
    //        receiveResponse("$ FETCH " + number + " body[text]\r\n");
            
    //    }
    //    bool CheckMessage(string msg) => !msg.Contains("Failure");
    //    string GetListOfInboxes()
    //    {
    //        if (Authed)
    //            return receiveResponse("$ LIST " + "\"\"" + " \"*\"" + "\r\n");
    //        else
    //            return "Not Authorised";
    //    }
       

    //    void Example(string[] args)
    //    {
    //        try
    //        {
    //            path = Environment.CurrentDirectory + "\\emailresponse.txt";

    //            if (System.IO.File.Exists(path))
    //                System.IO.File.Delete(path);

    //            sw = new System.IO.StreamWriter(System.IO.File.Create(path));
    //            // there should be no gap between the imap command and the \r\n       
    //            // ssl.read() -- while ssl.readbyte!= eof does not work because there is no eof from server 
    //            // cannot check for \r\n because in case of larger response from server ex:read email message 
    //            // there are lot of lines so \r \n appears at the end of each line 
    //            //ssl.timeout sets the underlying tcp connections timeout if the read or write 
    //            //time out exceeds then the undelying connection is closed 
    //            tcpc = new System.Net.Sockets.TcpClient("imap.gmail.com", 993);

    //            ssl = new System.Net.Security.SslStream(tcpc.GetStream());
    //            ssl.AuthenticateAsClient("imap.gmail.com");
    //            receiveResponse("");

    //            Console.WriteLine("username : ");
    //            username = Console.ReadLine();

    //            Console.WriteLine("password : ");
    //            password = Console.ReadLine();
    //            receiveResponse("$ LOGIN " + username + " " + password + "  \r\n");
    //            Console.Clear();

    //            receiveResponse("$ LIST " + "\"\"" + " \"*\"" + "\r\n");

    //            receiveResponse("$ SELECT INBOX\r\n");

    //            receiveResponse("$ STATUS INBOX (MESSAGES)\r\n");


    //            Console.WriteLine("enter the email number to fetch :");
    //            int number = int.Parse(Console.ReadLine());

    //            receiveResponse("$ FETCH " + number + " body[header]\r\n");
    //            receiveResponse("$ FETCH " + number + " body[text]\r\n");


    //            receiveResponse("$ LOGOUT\r\n");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("error: " + ex.Message);
    //        }
    //        finally
    //        {
    //            if (sw != null)
    //            {
    //                sw.Close();
    //                sw.Dispose();
    //            }
    //            if (ssl != null)
    //            {
    //                ssl.Close();
    //                ssl.Dispose();
    //            }
    //            if (tcpc != null)
    //            {
    //                tcpc.Close();
    //            }
    //        }
    //        Console.ReadKey();
    //    }

    //     string receiveResponse(string command)
    //    {
    //        try
    //        {
    //            if (command != "")
    //            {
    //                if (tcpc.Connected)
    //                {
    //                    dummy = Encoding.ASCII.GetBytes(command);
    //                    MimeMessage msg = MimeMessage.Load(tcpc.GetStream());
    //                    ssl.Write(dummy, 0, dummy.Length);
    //                }
    //                else
    //                {
    //                    throw new ApplicationException("TCP CONNECTION DISCONNECTED");
    //                }
    //            }
    //            ssl.Flush();


    //            buffer = new byte[2048];
    //            bytes = ssl.Read(buffer, 0, 2048);
    //            sb.Append(Encoding.ASCII.GetString(buffer));
    //            string s = sb.ToString();
    //            sw.WriteLine(s);
    //            sb = new StringBuilder();
    //            return s;

    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ApplicationException(ex.Message);
    //        }
    //    }
    //}
}
