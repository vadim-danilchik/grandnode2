using Grand.Domain;

namespace Shipping.Courier.Configurable.Domain
{
    public partial class ShippingCourierByWeightRecord : BaseEntity
    {
        public double WeightFrom { get; set; }

        public double WeightTo { get; set; }

        public double WeightRate { get; set; }
    }
}