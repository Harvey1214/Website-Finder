using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace WebsiteFinder
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public string FromAddress { get; set; }
        public string Password { get; set; }

        public List<string> ToAddresses { get; set; } = new List<string>();

        public void Send()
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(FromAddress);
            for (int i = 0; i < ToAddresses.Count; i++)
                mail.To.Add(ToAddresses[i]);
            mail.Subject = Subject;
            mail.Body = Body;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(FromAddress, Password);
            SmtpServer.EnableSsl = true;

            try
            {
                SmtpServer.Send(mail);
            }
            catch
            {
                
            }
        }
    }
}
