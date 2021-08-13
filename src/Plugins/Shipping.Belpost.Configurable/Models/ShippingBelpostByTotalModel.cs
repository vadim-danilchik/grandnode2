using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Belpost.Configurable.Models
{
    public class ShippingBelpostByTotalModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.TotalFrom")]
        public double TotalFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.TotalTo")]
        public double TotalTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Belpost.Configurable.Fields.TotalRate")]
        public double TotalRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}