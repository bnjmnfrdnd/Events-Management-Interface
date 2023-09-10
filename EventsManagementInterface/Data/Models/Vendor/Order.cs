using EventsManagementInterface.Data.Enums;

namespace EventsManagementInterface.Data.Models.Vendor
{
    public class Order : BaseModel
    {
        public int GuestIdentificationNumber { get; set; }  
        public string Name { get; set; }
        public double Price { get; set; }
        public OrderType Type { get; set; }
        public int Quantity { get; set; }
    }
}
