namespace Payments.WebpayCard.Models
{
    public class PaymentRedirectModel
    {
        public PaymentResponse data { get; set; }
    }

    public class PaymentResponse
    {
        public string wt { get; set; }

        public string redirectUrl { get; set; }
    }
}
