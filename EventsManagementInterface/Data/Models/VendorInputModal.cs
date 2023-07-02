namespace EventsManagementInterface.Data.Models
{
    public class VendorInputModal
    {
        public int GuestIdentificationNumber { get; set; }
        public string GuestName { get; set; }
        public string Title { get; set;}
        public string Message { get; set;}
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public List<string> TokensRemaining { get;set; }
    }
}
