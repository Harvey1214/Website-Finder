using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace WebsiteFinder
{
    public class Email
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string fromAddress { get; set; }
        public List<string> toAddresses { get; set; } = new List<string>();
        public List<Attachment> attachments { get; set; } = new List<Attachment>();

        public void SendAlert()
        {
            SmtpClient smtpClient = new SmtpClient("smtp-relay.sendinblue.com", 587);

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("mikuhoblik@gmail.com", "ISFwVCkLjGZYyWX4");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            mail.Subject = subject;
            mail.Body = body;
            
            foreach (Attachment attachment in attachments)
            {
                mail.Attachments.Add(attachment);
            }

            //Setting From , To and CC
            mail.From = new MailAddress(fromAddress);
            
            foreach (string toAddress in toAddresses)
            {
                mail.To.Add(toAddress);
            }

            smtpClient.Send(mail);
        }
    }
}
