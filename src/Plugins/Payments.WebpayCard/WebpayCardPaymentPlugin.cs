using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Plugins;
using System.Threading.Tasks;

namespace Payments.WebpayCard
{
    /// <summary>
    /// WebpayCard payment processor
    /// </summary>
    public class WebpayCardPaymentPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public WebpayCardPaymentPlugin(
            ITranslationService translationService,
            ILanguageService languageService,
            ISettingService settingService)
        {
            _translationService = translationService;
            _languageService = languageService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return WebpayCardPaymentDefaults.ConfigurationUrl;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task Install()
        {
            //settings
            await _settingService.SaveSetting(new WebpayCardPaymentSettings
            {
                UseSandbox = true
            });

            //locales
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.WebpayCard.FriendlyName", "Webpay Card");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.CurrencyId", "Currency Id");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.CurrencyId.Hint", "Specify your currency.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.Version", "Webpay version");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.Version.Hint", "Specify your webpay version.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.SecretKey", "Secret key");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.SecretKey.Hint", "Specify your secret key.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.RedirectionTip", "You will be redirected to Webpay site to complete the order.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.UseSandbox", "Use Sandbox");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.StoreId", "Store Id");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.StoreId.Hint", "Specify Store Id from portal.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.PaymentMethodDescription", "Card online");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.DisplayOrder", "Display order");


            await base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<WebpayCardPaymentSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Payments.WebpayCard.FriendlyName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.AdditionalFeePercentage.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.CurrencyId");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.CurrencyId.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.Version");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.Version.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.SecretKey");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.SecretKey.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.RedirectionTip");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.UseSandbox");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.UseSandbox.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.StoreId");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.StoreId.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.Fields.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.WebpayCard.PaymentMethodDescription");

            await base.Uninstall();
        }

        #endregion

    }
}