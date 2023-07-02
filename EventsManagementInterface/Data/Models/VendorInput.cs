namespace EventsManagementInterface.Data.Models
{
    public class VendorInput
    {
        public int Id { get; set; }
        public int GuestIdentificationNumber { get; set; }
        public int AlcoholicDrinkCounter { get; set; }
        public int NonAlcoholicDrinkCounter { get; set; }
        public int FoodCounter { get; set; }
        public int VendorId { get; set; }
    }
}