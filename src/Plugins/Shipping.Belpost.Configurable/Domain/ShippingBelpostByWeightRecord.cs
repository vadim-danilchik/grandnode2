using Grand.Domain;

namespace Shipping.Belpost.Configurable.Domain
{
    public partial class ShippingBelpostByWeightRecord : BaseEntity
    {
        public double WeightFrom { get; set; }

        public double WeightTo { get; set; }

        public double WeightRate { get; set; }
    }
}