using EventsManagementInterface.Data.Models;

namespace EventsManagementInterface.Data.Models
{
    public class Log : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
    }
}