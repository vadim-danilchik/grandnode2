using Grand.Domain;

namespace Shipping.Europost.Configurable.Domain
{
    public partial class ShippingEuropostByWeightRecord : BaseEntity
    {
        public double WeightFrom { get; set; }

        public double WeightTo { get; set; }

        public double WeightRate { get; set; }
    }
}