using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;

namespace Payments.WebpayCard.Controllers
{
    [ViewComponent(Name = "PaymentWebpayCard")]
    public class PaymentWebpayCardViewComponent : BaseViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(this.GetViewPath());
        }
    }
}