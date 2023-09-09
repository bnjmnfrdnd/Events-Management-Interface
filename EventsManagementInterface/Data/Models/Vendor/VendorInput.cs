namespace EventsManagementInterface.Data.Models.Vendor
{
    public class VendorInput : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }
        public int AlcoholicDrinkToken { get; set; }
        public int NonAlcoholicDrinkToken { get; set; }
        public int FoodToken { get; set; }
    }
}