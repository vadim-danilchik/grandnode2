using System.Collections.Generic;

namespace Payments.WebpayCard.Models
{
    public class PaymentRequestModel
    {
        public int wsb_storeid { get; set; }

        public string wsb_store { get; set; }

        public string wsb_currency_id { get; set; }

        public int wsb_version { get; set; }

        public string wsb_seed { get; set; }

        public int wsb_test { get; set; }

        public string wsb_order_num { get; set; }

        public string wsb_return_url { get; set; }

        public string wsb_notify_url { get; set; }

        public string wsb_cancel_return_url { get; set; }

        public string wsb_customer_name { get; set; }

        public string wsb_customer_address { get; set; }

        public string wsb_email { get; set; }

        public string wsb_phone { get; set; }

        public string wsb_shipping_name { get; set; }

        public List<string> wsb_invoice_item_name { get; set; }

        public List<int> wsb_invoice_item_quantity { get; set; }

        public List<double> wsb_invoice_item_price { get; set; }

        public double wsb_shipping_price { get; set; }

        public double wsb_discount_price { get; set; }

        public double wsb_tax { get; set; }

        public double wsb_total { get; set; }

        public string wsb_signature { get; set; }
    }
}
