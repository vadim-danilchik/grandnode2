using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Domain.Orders;
using Grand.Infrastructure.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipping.Courier.Configurable
{
    public class CourierShippingPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        #endregion

        #region Ctor
        public CourierShippingPlugin(
            ITranslationService translationService,
            ILanguageService languageService)
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
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.Courier.Configurable.FriendlyName", "Shipping Courier");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightFrom", "Order weight from");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightTo", "Order weight to");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightRate", "Rate per weight");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalFrom", "Order total from");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalTo", "Order total to");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalRate", "Rate total");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.AddRecord", "Add record");

            await base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task Uninstall()
        {
            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.Courier.Configurable.FriendlyName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightFrom");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightTo");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.WeightRate");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalFrom");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalTo");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.Fields.TotalRate");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Shipping.Courier.Configurable.AddRecord");

            await base.Uninstall();
        }

        /// <summary>
        /// Returns a value indicating whether shipping methods should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public async Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this shipping methods if all products in the cart are downloadable
            //or hide this shipping methods if current customer is from certain country
            return await Task.FromResult(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return CourierShippingDefaults.ConfigurationUrl;
        }

        #endregion
    }

}
