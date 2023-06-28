namespace EventsManagementInterface.Data.Models
{
    public class Attendee : Registration
    {
        public int FoodTokenAllowance { get; set; } = 10;
        public int DrinkTokenAllowance { get; set; } = 10;
        public int GuestIdentificationNumber { get; set; }
        public bool GuestIdentificationNumberEmailSent { get; set; }
    }
}
