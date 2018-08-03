using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;

namespace StiffLibrary
{
    //VideoSobre: https://www.youtube.com/watch?v=4lzZ0wzEK14
    public static class Mailing
    {
        private static NetworkCredential login;
        private static SmtpClient client;
        private static MailMessage msg;

        public static void Credentials(string username, string password)
        {
            string[] cut = username.Split('@');
            //USE OR DON'T USE THE @domain.com
            login = new NetworkCredential(username, password);
        }

        /*
        GMAIL = smtp.gmail.com; 587* ou 465* ou 25; True
        HOTMAIL = smtp.live.com; 465* ou 25; True
        */
        public static void Client(string smtpServer, int port, bool SSL, SendCompletedEventHandler CallBackDelegate = null)
        {
            client = new SmtpClient(smtpServer);
            client.Port = port;
            client.EnableSsl = SSL;
            if(CallBackDelegate != null)
                client.SendCompleted += CallBackDelegate;
            else
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallBack);
            
        }

        public static void Send(string username, string displayName, object body, string[] to, string subject, bool isHTML = true)
        {
            client.Credentials = login;
            client.UseDefaultCredentials = true;
            msg = new MailMessage { From = new MailAddress(username, displayName) };
            foreach(string tos in to)
            {
                msg.To.Add(new MailAddress(tos));
            }
            msg.Subject = subject;
            msg.Body = (string)body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = isHTML;
            msg.Priority = MailPriority.Normal;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            string userstate = "Sending...";
            client.SendAsync(msg, userstate);
        }

        public static void SendCompletedCallBack(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                LastError = string.Format("{0} send canceled.", e.UserState);
            if (e.Error != null)
                LastError = string.Format("{0} {1}", e.UserState, e.Error);
            else
                LastError = "Your email has been successfully sent.";
        }

        public static string LastError = "";
    }
}
