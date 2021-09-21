using Grand.Domain;

namespace Shipping.Courier.Configurable.Domain
{
    public partial class ShippingCourierByTotalRecord : BaseEntity
    {
        public double TotalFrom { get; set; }

        public double TotalTo { get; set; }

        public double TotalRate { get; set; }
    }
}