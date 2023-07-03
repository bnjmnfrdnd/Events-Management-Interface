namespace EventsManagementInterface.Data.Models.Administration
{
    public class Log : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
    }
}