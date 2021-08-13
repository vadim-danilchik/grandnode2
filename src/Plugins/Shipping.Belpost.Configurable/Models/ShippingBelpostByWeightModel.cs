using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Belpost.Configurable.Models
{
    public class ShippingBelpostByWeightModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.WeightFrom")]
        public double WeightFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.WeightTo")]
        public double WeightTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.WeightRate")]
        public double WeightRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public string BaseWeightIn { get; set; }
    }
}