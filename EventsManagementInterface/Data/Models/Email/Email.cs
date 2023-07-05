using System.Net.Mail;

namespace EventsManagementInterface.Data.Models.Email
{
    public class Email
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string HTMLMessage { get; set; }
        public string CredentialsEmail { get; set; } = "bg.f@hotmail.co.uk";
        public string CredentialsPassword { get; set; } = "Pokemon1!";
        public string SMTPServer { get; set; } = "smtp.office365.com";
    }
}