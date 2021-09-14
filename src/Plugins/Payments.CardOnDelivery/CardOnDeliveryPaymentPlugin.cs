using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Plugins;
using System.Threading.Tasks;

namespace Payments.CardOnDelivery
{
    /// <summary>
    /// CardOnDelivery payment processor
    /// </summary>
    public class CardOnDeliveryPaymentPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public CardOnDeliveryPaymentPlugin(
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
            return CardOnDeliveryPaymentDefaults.ConfigurationUrl;
        }

        public override async Task Install()
        {
            var settings = new CardOnDeliveryPaymentSettings
            {
                DescriptionText = ""
            };
            await _settingService.SaveSetting(settings);
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.CardOnDelivery.FriendlyName", "Card on delivery");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.DescriptionText", "Description");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.PaymentMethodDescription", "Card On Delivery");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFee.Hint", "The additional fee.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.ShippableProductRequired", "Shippable product required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.DisplayOrder", "Display order");


            await base.Install();
        }

        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<CardOnDeliveryPaymentSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.DescriptionText");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.DescriptionText.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.PaymentMethodDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.AdditionalFeePercentage.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.ShippableProductRequired");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payment.CardOnDelivery.ShippableProductRequired.Hint");

            await base.Uninstall();
        }

        #endregion


    }
}