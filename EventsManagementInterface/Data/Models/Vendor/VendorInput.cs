namespace EventsManagementInterface.Data.Models.Vendor
{
    public class VendorInput
    {
        public int Id { get; set; }
        public int GuestIdentificationNumber { get; set; }
        public int AlcoholicDrinkToken { get; set; }
        public int NonAlcoholicDrinkToken { get; set; }
        public int FoodToken { get; set; }
        public int VendorId { get; set; }
        public string DrinkName { get; set;}
        public string DrinkPrice { get; set;}
    }
}