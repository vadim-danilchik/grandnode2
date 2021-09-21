using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.Courier.Configurable.Models
{
    public class ShippingCourierByTotalModel : BaseEntityModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.TotalFrom")]
        public double TotalFrom { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.TotalTo")]
        public double TotalTo { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Courier.Configurable.Fields.TotalRate")]
        public double TotalRate { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}