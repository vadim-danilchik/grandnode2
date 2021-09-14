using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Catalog.Interfaces.Tax;
using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Interfaces.CheckoutAttributes;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Utilities;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Customers.Interfaces;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Payments.WebpayCard.Models;
using Payments.WebpayCard.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payments.WebpayCard
{
    public class WebpayCardPaymentProvider : IPaymentProvider
    {

        private readonly ITranslationService _translationService;
        private readonly WebpayCardPaymentSettings _webpayCardPaymentSettings;

        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IUserFieldService _userFieldService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITaxService _taxService;
        private readonly IProductService _productService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IWebpayHttpClient _webpayHttpClient;

        #region Ctor

        public WebpayCardPaymentProvider(
            ICheckoutAttributeParser checkoutAttributeParser,
            IUserFieldService userFieldService,
            IHttpContextAccessor httpContextAccessor,
            ITranslationService translationService,
            ITaxService taxService,
            IProductService productService,
            IServiceProvider serviceProvider,
            IWorkContext workContext,
            IOrderService orderService,
            WebpayCardPaymentSettings webpayCardPaymentSettings,
            IWebpayHttpClient webpayHttpClient)
        {
            _checkoutAttributeParser = checkoutAttributeParser;
            _userFieldService = userFieldService;
            _httpContextAccessor = httpContextAccessor;
            _translationService = translationService;
            _taxService = taxService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _workContext = workContext;
            _orderService = orderService;
            _webpayCardPaymentSettings = webpayCardPaymentSettings;
            _webpayHttpClient = webpayHttpClient;
        }

        #endregion

        public string ConfigurationUrl => WebpayCardPaymentDefaults.ConfigurationUrl;

        public string SystemName => WebpayCardPaymentDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(WebpayCardPaymentDefaults.FriendlyName);

        public int Priority => _webpayCardPaymentSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();


        #region Utilities

        private async Task<PaymentRequestModel> CreateQueryParameters(Order order)
        {
            //get store location
            var storeLocation = _workContext.CurrentStore.SslEnabled ? _workContext.CurrentStore.SecureUrl.TrimEnd('/') : _workContext.CurrentStore.Url.TrimEnd('/');
            var stateProvince = "";
            var countryName = "";
            if (!string.IsNullOrEmpty(order.ShippingAddress?.StateProvinceId))
            {
                var countryService = _serviceProvider.GetRequiredService<ICountryService>();
                var country = await countryService.GetCountryById(order.ShippingAddress?.CountryId);
                var state = country?.StateProvinces.FirstOrDefault(x => x.Id == order.ShippingAddress?.StateProvinceId);
                if (state != null)
                    stateProvince = state.Name;
            }
            if (!string.IsNullOrEmpty(order.ShippingAddress?.CountryId))
            {
                var country = await _serviceProvider.GetRequiredService<ICountryService>().GetCountryById(order.ShippingAddress?.CountryId);
                if (country != null)
                    countryName = country.Name;
            }

            var address = new StringBuilder();
            address.Append(countryName);
            address.Append(", ");
            address.Append(stateProvince);
            if (order.ShippingAddress?.City != null)
            {
                address.Append(", ");
                address.Append(order.ShippingAddress?.City);
            }
            if (order.ShippingAddress?.Address1 != null)
            {
                address.Append(", ");
                address.Append(order.ShippingAddress?.Address1);
            }

            var names = new List<string>();
            var quantities = new List<int>();
            var prices = new List<double>();

            //add shopping cart items
            foreach (var item in order.OrderItems)
            {
                var product = await _productService.GetProductById(item.ProductId);
                var roundedItemPrice = Math.Round(item.UnitPriceExclTax, 2);

                //add query parameters
                names.Add(product.Name);
                prices.Add(roundedItemPrice);
                quantities.Add(item.Quantity);
            }

            //add checkout attributes as order items
            var checkoutAttributeValues = await _checkoutAttributeParser.ParseCheckoutAttributeValue(order.CheckoutAttributes);
            var currencyService = _serviceProvider.GetRequiredService<ICurrencyService>();
            var workContext = _serviceProvider.GetRequiredService<IWorkContext>();
            var customer = await _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerById(order.CustomerId);
            foreach (var attributeValue in checkoutAttributeValues)
            {
                var attributePrice = await _taxService.GetCheckoutAttributePrice(attributeValue.ca, attributeValue.cav, false, customer);
                if (attributePrice.checkoutPrice > 0)
                {
                    double roundedAttributePrice = Math.Round(await currencyService.ConvertFromPrimaryStoreCurrency(attributePrice.checkoutPrice, workContext.WorkingCurrency), 2);
                    //add query parameters
                    if (attributeValue.ca != null)
                    {
                        names.Add(attributeValue.ca.Name);
                        prices.Add(roundedAttributePrice);
                        quantities.Add(1);
                    }
                }
            }

            //add payment method additional fee as a separate order item, if it has price
            var roundedPaymentMethodPrice = Math.Round(order.PaymentMethodAdditionalFeeExclTax, 2);
            if (roundedPaymentMethodPrice > 0)
            {
                names.Add("Дополнительный сбор за оплату");
                prices.Add(roundedPaymentMethodPrice);
                quantities.Add(1);
            }

            //round order total
            var roundedOrderTotal = Math.Round(order.OrderTotal, 2);

            var wsb_seed = Guid.NewGuid().ToString();

            var signature = string.Concat(
                wsb_seed,
                _webpayCardPaymentSettings.StoreId.ToString(),
                order.OrderNumber.ToString(),
                _webpayCardPaymentSettings.UseSandbox ? "1" : "0",
                _webpayCardPaymentSettings.CurrencyId,
                roundedOrderTotal.ToString(CultureInfo.InvariantCulture),
                _webpayCardPaymentSettings.SecretKey);

            var sha1 = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(signature));
            var hash = string.Empty;
            foreach (var b in sha1)
            {
                hash += b.ToString("x2");
            }
            //create query parameters
            return new PaymentRequestModel
            {
                wsb_storeid = _webpayCardPaymentSettings.StoreId,
                wsb_store = _workContext.CurrentStore.CompanyName,
                wsb_currency_id = _webpayCardPaymentSettings.CurrencyId,
                wsb_version = _webpayCardPaymentSettings.Version,
                wsb_seed = wsb_seed,
                wsb_test = _webpayCardPaymentSettings.UseSandbox ? 1 : 0,

                //order identifier
                wsb_order_num = order.OrderNumber.ToString(),

                //return, notify and cancel URL
                wsb_return_url = $"{storeLocation}{WebpayHelper.WsbReturnUrl}",
                wsb_notify_url = $"{storeLocation}{WebpayHelper.WsbNotifyUrl}",
                wsb_cancel_return_url = $"{storeLocation}{WebpayHelper.WsbCancelReturnUrl}",

                //shipping address, if exists
                wsb_customer_name = order.ShippingAddress?.LastName + " " + order.ShippingAddress?.FirstName,
                wsb_customer_address = address.ToString(),
                wsb_email = order.ShippingAddress?.Email,
                wsb_phone = order.ShippingAddress?.PhoneNumber,
                wsb_shipping_name = order.ShippingMethod,

                wsb_invoice_item_name = names,
                wsb_invoice_item_quantity = quantities,
                wsb_invoice_item_price = prices,
                wsb_shipping_price = order.OrderShippingExclTax,
                wsb_discount_price = order.OrderDiscount,
                wsb_tax = order.OrderTax,

                wsb_total = roundedOrderTotal,
                wsb_signature = hash
            };
        }

        #endregion

        #region Properties

        public async Task<PaymentTransaction> InitPaymentTransaction()
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        public async Task<ProcessPaymentResult> ProcessPayment(PaymentTransaction paymentTransaction)
        {
            var result = new ProcessPaymentResult();
            return await Task.FromResult(result);
        }

        public Task PostProcessPayment(PaymentTransaction paymentTransaction)
        {
            //nothing
            return Task.CompletedTask;
        }

        public async Task PostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            var order = await _orderService.GetOrderByGuid(paymentTransaction.OrderGuid);
            var queryParameters = await CreateQueryParameters(order);

            var paymentUrl = await _webpayHttpClient.GetPaymentUrl(queryParameters);
            _httpContextAccessor.HttpContext.Response.Redirect(paymentUrl);
        }

        public async Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return await Task.FromResult(false);
        }

        public async Task<double> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            if (_webpayCardPaymentSettings.AdditionalFee <= 0)
                return _webpayCardPaymentSettings.AdditionalFee;

            double result;
            if (_webpayCardPaymentSettings.AdditionalFeePercentage)
            {
                //percentage
                var orderTotalCalculationService = _serviceProvider.GetRequiredService<IOrderCalculationService>();
                var subtotal = await orderTotalCalculationService.GetShoppingCartSubTotal(cart, true);
                result = (double)((((float)subtotal.subTotalWithDiscount) * ((float)_webpayCardPaymentSettings.AdditionalFee)) / 100f);
            }
            else
            {
                //fixed value
                result = _webpayCardPaymentSettings.AdditionalFee;
            }
            if (result > 0)
            {
                var currencyService = _serviceProvider.GetRequiredService<ICurrencyService>();
                result = await currencyService.ConvertFromPrimaryStoreCurrency(result, _workContext.WorkingCurrency);
            }
            //return result;
            return await Task.FromResult(result);
        }

        public async Task<CapturePaymentResult> Capture(PaymentTransaction paymentTransaction)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return await Task.FromResult(result);
        }

        public async Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return await Task.FromResult(result);
        }

        public async Task<VoidPaymentResult> Void(PaymentTransaction paymentTransaction)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return await Task.FromResult(result);
        }

        public Task CancelPayment(PaymentTransaction paymentTransaction)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> CanRePostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null)
                throw new ArgumentNullException(nameof(paymentTransaction));

            //ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - paymentTransaction.CreatedOnUtc).TotalSeconds < 15)
                return false;

            return await Task.FromResult(true);
        }

        public async Task<IList<string>> ValidatePaymentForm(IFormCollection form)
        {
            return await Task.FromResult(new List<string>());
        }

        public async Task<PaymentTransaction> SavePaymentInfo(IFormCollection form)
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        public async Task<bool> SupportCapture()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> SupportPartiallyRefund()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> SupportRefund()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> SupportVoid()
        {
            return await Task.FromResult(false);
        }

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public async Task<bool> SkipPaymentInfo()
        {
            return await Task.FromResult(false);
        }

        public async Task<string> Description()
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to Webpay site to complete the payment"
            return await Task.FromResult(_translationService.GetResource("Plugins.Payments.WebpayCard.PaymentMethodDescription"));
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentWebpayCard";
        }

        public string LogoURL => "/Plugins/Payments.WebpayCard/logo.jpg";

        #endregion
    }
}
