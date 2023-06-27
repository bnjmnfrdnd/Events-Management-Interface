namespace EventsManagementInterface.Data.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now.Date;
        public bool Archived { get; set; }
    }
}
