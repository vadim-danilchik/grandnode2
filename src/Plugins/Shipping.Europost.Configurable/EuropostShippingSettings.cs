using Grand.Domain.Configuration;

namespace Shipping.Europost.Configurable
{
    public class EuropostShippingSettings : ISettings
    {
        public int DisplayOrder => 2;

        public string UpdateRequestUrl { get; set; }

        public string UpdateRequestBody { get; set; }

        public string MapUrl { get; set; }
    }
}
