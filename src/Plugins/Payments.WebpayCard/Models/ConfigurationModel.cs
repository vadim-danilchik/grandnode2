using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Payments.WebpayCard.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.StoreId")]
        public int StoreId { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.CurrencyId")]
        public string CurrencyId { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.Version")]
        public int Version { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.SecretKey")]
        public string SecretKey { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.AdditionalFee")]
        public double AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.WebpayCard.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }



    }
}