using Microsoft.AspNetCore.Http;

namespace Checkout.OnePage.Models
{
    public class ConfirmOrderModel
    {
        public IFormCollection ShippingForm { get; set; }

        public IFormCollection ShippingMethodForm { get; set; }

        public IFormCollection PaymentMethodForm { get; set; }

        public IFormCollection PaymentInfoForm { get; set; }
    }
}
