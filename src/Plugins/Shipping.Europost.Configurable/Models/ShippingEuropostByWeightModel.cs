using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Europost.Configurable.Models
{
    public class ShippingEuropostByWeightModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.WeightFrom")]
        public double WeightFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.WeightTo")]
        public double WeightTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.WeightRate")]
        public double WeightRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public string BaseWeightIn { get; set; }
    }
}