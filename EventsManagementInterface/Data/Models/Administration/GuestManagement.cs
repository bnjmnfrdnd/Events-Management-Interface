namespace EventsManagementInterface.Data.Models.Administration
{
    public class GuestManagement
    {
        public int GuestIdentificationNumber { get; set; }
        public int? FoodTokenAllowance { get; set; }
        public int? AlcoholicDrinkTokenAllowance { get; set; }
        public int? NonAlcoholicDrinkTokenAllowance { get; set; }
    }
}