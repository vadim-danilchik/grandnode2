using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Plugins;
using System.Threading.Tasks;

namespace Payments.PostOnDelivery
{
    /// <summary>
    /// PostOnDelivery payment processor
    /// </summary>
    public class PostOnDeliveryPaymentPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public PostOnDeliveryPaymentPlugin(
            ISettingService settingService,
            ITranslationService translationService,
            ILanguageService languageService)
        {
            _settingService = settingService;
            _translationService = translationService;
            _languageService = languageService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return PostOnDeliveryPaymentDefaults.ConfigurationUrl;
        }

        public override async Task Install()
        {
            var settings = new PostOnDeliveryPaymentSettings
            {
                DescriptionText = ""
            };
            await _settingService.SaveSetting(settings);
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.PostOnDelivery.FriendlyName", "Post on delivery");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.DescriptionText", "Description");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.PaymentMethodDescription", "Post On Delivery");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFee.Hint", "The additional fee.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.ShippableProductRequired", "Shippable product required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.DisplayOrder", "Display order");


            await base.Install();
        }

        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<PostOnDeliveryPaymentSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.DescriptionText");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.DescriptionText.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.PaymentMethodDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.AdditionalFeePercentage.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.ShippableProductRequired");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.PostOnDelivery.ShippableProductRequired.Hint");

            await base.Uninstall();
        }

        #endregion


    }
}