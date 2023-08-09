namespace EventsManagementInterface.Data.Models.Vendor
{
    public class Order
    {
        public int id { get; set; }
        public DateTime OrderDate { get; set; }
        public bool Archived { get; set; }
    }
}
