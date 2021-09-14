using Grand.Business.Common.Interfaces.Configuration;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;
using Payments.PostOnDelivery.Models;

namespace Payments.PostOnDelivery.Components
{
    public class PaymentPostOnDeliveryViewComponent : BaseViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public PaymentPostOnDeliveryViewComponent(
            IWorkContext workContext,
            ISettingService settingService)
        {
            _workContext = workContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke()
        {
            var PostOnDeliveryPaymentSettings = _settingService.LoadSetting<PostOnDeliveryPaymentSettings>(_workContext.CurrentStore.Id);

            var model = new PaymentInfoModel {
                DescriptionText = PostOnDeliveryPaymentSettings.DescriptionText
            };
            return View(this.GetViewPath(), model);
        }
    }
}