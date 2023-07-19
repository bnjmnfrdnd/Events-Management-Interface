using System.Net.Mail;

namespace EventsManagementInterface.Data.Models.Email
{
    public class Email
    {
        public string Recipient { get; set; } = "benjaminferdinand@gmail.com"; //Default until overwritten
        public string Subject { get; set; }
        public string HTMLMessage { get; set; }
        public string CredentialsEmail { get; set; } = "coloplastevents@outlook.com";
        public string CredentialsPassword { get; set; } = "Orangerycyclonesneezesubvention2!";
        public string SMTPServer { get; set; } = "smtp.office365.com";
    }
}