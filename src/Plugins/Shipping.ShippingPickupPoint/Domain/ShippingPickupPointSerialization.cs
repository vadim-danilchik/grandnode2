namespace Shipping.ShippingPickupPoint.Domain
{
    public class ShippingPickupPointSerializable
    {
        public string Id { get; set; }
        public string ShippingPickupPointName { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string ZipPostalCode { get; set; }
        public double PickupFee { get; set; }
        public string OpeningHours { get; set; }
        public string StoreId { get; set; }
    }
}