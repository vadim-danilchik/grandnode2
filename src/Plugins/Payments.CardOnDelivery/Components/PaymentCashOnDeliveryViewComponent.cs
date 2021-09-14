using Grand.Business.Common.Interfaces.Configuration;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;
using Payments.CardOnDelivery.Models;

namespace Payments.CardOnDelivery.Components
{
    public class PaymentCardOnDeliveryViewComponent : BaseViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public PaymentCardOnDeliveryViewComponent(
            IWorkContext workContext,
            ISettingService settingService)
        {
            _workContext = workContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke()
        {
            var CardOnDeliveryPaymentSettings = _settingService.LoadSetting<CardOnDeliveryPaymentSettings>(_workContext.CurrentStore.Id);

            var model = new PaymentInfoModel {
                DescriptionText = CardOnDeliveryPaymentSettings.DescriptionText
            };
            return View(this.GetViewPath(), model);
        }
    }
}