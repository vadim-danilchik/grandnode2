using Grand.Domain;

namespace Shipping.Belpost.Configurable.Domain
{
    public partial class ShippingBelpostByTotalRecord : BaseEntity
    {
        public double TotalFrom { get; set; }

        public double TotalTo { get; set; }

        public double TotalRate { get; set; }
    }
}