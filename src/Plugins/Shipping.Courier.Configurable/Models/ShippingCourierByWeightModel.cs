using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Courier.Configurable.Models
{
    public class ShippingCourierByWeightModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.WeightFrom")]
        public double WeightFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.WeightTo")]
        public double WeightTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.WeightRate")]
        public double WeightRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public string BaseWeightIn { get; set; }
    }
}