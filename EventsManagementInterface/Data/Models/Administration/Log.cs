using EventsManagementInterface.Data.Enums;

namespace EventsManagementInterface.Data.Models.Administration
{
    public class Log : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }
        public LogType Type { get; set; }
        public string Summary { get; set; }
        public int TokensUsed {  get; set; }
    }
}