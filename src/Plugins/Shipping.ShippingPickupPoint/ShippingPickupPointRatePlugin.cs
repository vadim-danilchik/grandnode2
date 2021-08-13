using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Plugins;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint
{
    public class ShippingPickupPointRatePlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor
        public ShippingPickupPointRatePlugin(
            ITranslationService translationService,
            ILanguageService languageService
            )
        {
            _translationService = translationService;
            _languageService = languageService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task Install()
        {
            //locales       
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.FriendlyName", "Shipping Point");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.PluginName", "Shipping Point");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.PluginDescription", "Choose a place where you can pick up your order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.ShippingPickupPointName", "Point Name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Description", "Description");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.PickupFee", "Pickup Fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.OpeningHours", "Open Between");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Store", "Store Name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.City", "City");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Address1", "Address 1");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.ZipPostalCode", "Zip postal code");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Country", "Country");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.MethodAndFee", "{0}");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.AddNew", "Add New Point");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredShippingPickupPointName", "Shipping Point Name Is Required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredDescription", "Description Is Required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredOpeningHours", "Opening Hours Are Required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.SelectShippingOption", "Select Shipping Option");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.ChooseShippingPickupPoint", "Choose Shipping Point");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.SelectBeforeProceed", "Select Shipping Option Before Proceed");

            await base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task Uninstall()
        {
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.PluginName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.PluginDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.ShippingPickupPointName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Description");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.PickupFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.OpeningHours");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Store");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.AddNew");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredShippingPickupPointName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.RequiredOpeningHours");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.SelectShippingOption");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.ChooseShippingPickupPoint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.SelectBeforeProceed");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.City");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Address1");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.ZipPostalCode");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.Fields.Country");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.ShippingPickupPoint.MethodAndFee");


            await base.Uninstall();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return ShippingPickupPointRateDefaults.ConfigurationUrl;
        }

        #endregion

    }
}
