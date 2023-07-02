namespace EventsManagementInterface.Data.Models
{
    public class Attendee : Registration
    {
        public int FoodTokenAllowance { get; set; } = 10;
        public int AlcoholicDrinkTokenAllowance { get; set; } = 10;
        public int NonAlcoholicDrinkTokenAllowance { get; set; } = 10;
        public int GuestIdentificationNumber { get; set; }
        public bool GuestIdentificationNumberEmailSent { get; set; }
    }
}
