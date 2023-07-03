namespace EventsManagementInterface.Data.Models.Administration
{
    public class AdministrationModal
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int GuestIdentificationNumber { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
