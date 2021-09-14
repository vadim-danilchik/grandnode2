namespace Payments.WebpayCard.Models
{
    public class PaymentNotifyModel
    {
        public string batch_timestamp { get; set; }

        public string currency_id { get; set; }

        public string amount { get; set; }
        //
        public string payment_method { get; set; }

        public string order_id { get; set; }

        public string site_order_id { get; set; }

        public string transaction_id { get; set; }
        //
        public string payment_type { get; set; }

        public string rrn { get; set; }
        //
        public string wsb_signature { get; set; }

        public string action { get; set; }

        public string rc { get; set; }

        public string approval { get; set; }
    }
}
