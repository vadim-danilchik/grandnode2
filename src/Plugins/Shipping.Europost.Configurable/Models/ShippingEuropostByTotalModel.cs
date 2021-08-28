using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Europost.Configurable.Models
{
    public class ShippingEuropostByTotalModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.TotalFrom")]
        public double TotalFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.TotalTo")]
        public double TotalTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.TotalRate")]
        public double TotalRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}