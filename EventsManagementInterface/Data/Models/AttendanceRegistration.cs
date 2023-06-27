namespace EventsManagementInterface.Data.Models
{
    public class AttendanceRegistration : BaseModel
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
