namespace EventsManagementInterface.Data.Models
{
    public class RegistrationModal
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string GuestIdentificationNumber { get; set; }
        public List<string> Errors { get; set; }
    }
}
