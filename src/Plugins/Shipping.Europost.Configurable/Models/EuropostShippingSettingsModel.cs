using Grand.Infrastructure.ModelBinding;

namespace Shipping.Europost.Configurable.Models
{
    public class EuropostShippingSettingsModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.UpdateRequestUrl")]
        public string UpdateRequestUrl { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.UpdateRequestBody")]
        public string UpdateRequestBody { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.Europost.Configurable.Fields.MapUrl")]
        public string MapUrl { get; set; }
    }
}
