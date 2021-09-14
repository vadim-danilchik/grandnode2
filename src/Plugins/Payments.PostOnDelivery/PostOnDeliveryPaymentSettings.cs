using Grand.Domain.Configuration;

namespace Payments.PostOnDelivery
{
    public class PostOnDeliveryPaymentSettings : ISettings
    {
        public int DisplayOrder { get; set; }

        public string DescriptionText { get; set; }

        public bool AdditionalFeePercentage { get; set; }

        public double AdditionalFee { get; set; }

        public bool ShippableProductRequired { get; set; }

        public bool SkipPaymentInfo { get; set; }
    }
}
