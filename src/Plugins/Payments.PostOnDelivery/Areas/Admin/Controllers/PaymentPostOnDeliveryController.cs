using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Stores;
using Grand.Business.Common.Services.Security;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Infrastructure;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.PostOnDelivery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.PostOnDelivery.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.PaymentMethods)]
    public class PaymentPostOnDeliveryController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;


        public PaymentPostOnDeliveryController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ITranslationService translationService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _translationService = translationService;
        }


        protected virtual async Task<string> GetActiveStore(IStoreService storeService, IWorkContext workContext)
        {
            var stores = await storeService.GetAllStores();
            if (stores.Count < 2)
                return stores.FirstOrDefault().Id;

            var storeId = workContext.CurrentCustomer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.AdminAreaStoreScopeConfiguration);
            var store = await storeService.GetStoreById(storeId);

            return store != null ? store.Id : "";
        }

        public async Task<IActionResult> Configure()
        {
            //load settings for a chosen store scope
            var storeScope = await this.GetActiveStore(_storeService, _workContext);
            var PostOnDeliveryPaymentSettings = _settingService.LoadSetting<PostOnDeliveryPaymentSettings>(storeScope);

            var model = new ConfigurationModel {
                DescriptionText = PostOnDeliveryPaymentSettings.DescriptionText,
                AdditionalFee = PostOnDeliveryPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = PostOnDeliveryPaymentSettings.AdditionalFeePercentage,
                ShippableProductRequired = PostOnDeliveryPaymentSettings.ShippableProductRequired,
                DisplayOrder = PostOnDeliveryPaymentSettings.DisplayOrder
            };
            model.DescriptionText = PostOnDeliveryPaymentSettings.DescriptionText;

            model.ActiveStore = storeScope;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await this.GetActiveStore(_storeService, _workContext);
            var PostOnDeliveryPaymentSettings = _settingService.LoadSetting<PostOnDeliveryPaymentSettings>(storeScope);

            //save settings
            PostOnDeliveryPaymentSettings.DescriptionText = model.DescriptionText;
            PostOnDeliveryPaymentSettings.AdditionalFee = model.AdditionalFee;
            PostOnDeliveryPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            PostOnDeliveryPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;
            PostOnDeliveryPaymentSettings.DisplayOrder = model.DisplayOrder;

            await _settingService.SaveSetting(PostOnDeliveryPaymentSettings, storeScope);

            //now clear settings cache
            await _settingService.ClearCache();

            Success(_translationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

    }
}
