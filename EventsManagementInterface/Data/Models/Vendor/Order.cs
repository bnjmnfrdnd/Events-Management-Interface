namespace EventsManagementInterface.Data.Models.Vendor
{
    public class Order : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }  
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Size { get; set; }
    }
}
