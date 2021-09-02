using Grand.Infrastructure.Models;
using Grand.Web.Models.Checkout;

namespace Checkout.OnePage.Models
{
    public partial class OnePageCheckoutModel : BaseModel
    {
        public bool ShippingRequired { get; set; }

        public CheckoutShippingAddressModel ShippingAddress { get; set; }

        public CheckoutShippingMethodModel ShippingMethod { get; set; }

        public CheckoutPaymentMethodModel PaymentMethod { get; set; }
    }
}