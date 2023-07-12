using System.Net.Mail;
using Blazorise;
using EventsManagementInterface.Data.Models.Email;

namespace EventsManagementInterface.Data.Services
{
    public class Utility
    {
        public static bool SendEmail(Email email)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.CredentialsEmail);
            mail.To.Add(email.Recipient/*"benjaminferdinand@gmail.com"*/);
            mail.Subject = email.Subject;
            mail.Body = email.HTMLMessage.ToString();
            mail.IsBodyHtml = true;

            SmtpClient server = new SmtpClient();
            server.UseDefaultCredentials = false;
            server.Credentials = new System.Net.NetworkCredential(email.CredentialsEmail, email.CredentialsPassword);
            server.Port = 25;
            server.EnableSsl = true;
            server.Host = email.SMTPServer;
            server.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                server.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
