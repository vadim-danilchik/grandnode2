using Grand.Infrastructure.Models;

namespace Shipping.ShippingPickupPoint.Models
{
    public class PointModel : BaseEntityModel
    {
        public string ShippingPickupPointName { get; set; }
        public string Description { get; set; }
        public string OpeningHours { get; set; }
        public string PickupFee { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string ZipPostalCode { get; set; }
        public string CountryName { get; set; }
    }

}