using System;
using System.Globalization;
using System.Net.Mail;
using Blazorise;
using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Email;

namespace EventsManagementInterface.Data.Services
{
    public class Utility
    {
        public static bool SendEmail(Email email)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.CredentialsEmail);
            mail.To.Add(email.Recipient);
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
            catch (Exception exception)
            {
                return false;
            }
        }

        public static async Task<bool> SendEmailAsync(Email email)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.CredentialsEmail);
            mail.To.Add(email.Recipient);
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
                await server.SendMailAsync(mail);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public static bool SendExceptionThrownEmail(string function, Exception exception)
        {
            Email email = new Email();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.CredentialsEmail);
            mail.To.Add(email.Recipient);
            mail.Subject = $"An exception has been thrown - {function}.";
            mail.Body = $"<b>Message:</b> {exception.Message} " +
                    $"<br><br>" +
                    $"<b>Inner Exception:</b> {exception.InnerException}" +
                    $"<br><br>" +
                    $"<b>Stack trace:</b> {exception.StackTrace}";
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
                SendExceptionThrownEmail("SendExceptionThrownEmail", ex);
                return false;
            }
        }        
    }
}