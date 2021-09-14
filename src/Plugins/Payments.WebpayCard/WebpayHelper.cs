using Grand.Domain.Payments;

namespace Payments.WebpayCard
{
    public static class WebpayHelper
    {
        public static string WebpayUrlSandbox => "https://securesandbox.webpay.by/api/v1/payment";

        public static string WebpayUrl => "https://payment.webpay.by/api/v1/payment";

        public static string WsbReturnUrl => "/Plugins/PaymentWebpayCard/SuccessPayment";

        public static string WsbNotifyUrl => "/Plugins/PaymentWebpayCard/NotifyPayment";

        public static string WsbCancelReturnUrl => "/Plugins/PaymentWebpayCard/CancelPayment";

        public static PaymentStatus GetPaymentStatus(string payment_type)
        {
            var result = PaymentStatus.Pending;

            if (payment_type == null)
                payment_type = string.Empty;

            switch (payment_type.ToLowerInvariant())
            {
                case "3":
                    result = PaymentStatus.Pending;
                    break;
                case "1":
                case "4":
                case "10":
                    result = PaymentStatus.Paid;
                    break;
                case "7":
                case "9":
                    result = PaymentStatus.Voided;
                    break;
                case "5":
                    result = PaymentStatus.PartiallyRefunded;
                    break;
                case "11":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}

