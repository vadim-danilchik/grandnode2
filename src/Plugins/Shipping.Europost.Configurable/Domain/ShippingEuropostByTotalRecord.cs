using Grand.Domain;

namespace Shipping.Europost.Configurable.Domain
{
    public partial class ShippingEuropostByTotalRecord : BaseEntity
    {
        public double TotalFrom { get; set; }

        public double TotalTo { get; set; }

        public double TotalRate { get; set; }
    }
}