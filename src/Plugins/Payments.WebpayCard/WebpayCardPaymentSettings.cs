using Grand.Domain.Configuration;

namespace Payments.WebpayCard
{
    public class WebpayCardPaymentSettings : ISettings
    {
        public int StoreId { get; set; }

        public string CurrencyId { get; set; }

        public int Version { get; set; }

        public string SecretKey { get; set; }

        public bool UseSandbox { get; set; }

        public bool AdditionalFeePercentage { get; set; }

        public double AdditionalFee { get; set; }

        public int DisplayOrder { get; set; }
        
    }
}
