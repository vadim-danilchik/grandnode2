﻿using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Extensions;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Utilities;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments.CardOnDelivery
{
    public class CardOnDeliveryPaymentProvider : IPaymentProvider
    {
        private readonly ITranslationService _translationService;
        private readonly IServiceProvider _serviceProvider;

        private readonly CardOnDeliveryPaymentSettings _CardOnDeliveryPaymentSettings;

        public CardOnDeliveryPaymentProvider(
            ITranslationService translationService,
            IServiceProvider serviceProvider,
            CardOnDeliveryPaymentSettings CardOnDeliveryPaymentSettings
            )
        {
            _translationService = translationService;
            _serviceProvider = serviceProvider;
            _CardOnDeliveryPaymentSettings = CardOnDeliveryPaymentSettings;
        }

        public string ConfigurationUrl => CardOnDeliveryPaymentDefaults.ConfigurationUrl;

        public string SystemName => CardOnDeliveryPaymentDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(CardOnDeliveryPaymentDefaults.FriendlyName);

        public int Priority => _CardOnDeliveryPaymentSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        /// <summary>
        /// Init a process a payment transaction
        /// </summary>
        /// <returns>Payment transaction</returns>
        public async Task<PaymentTransaction> InitPaymentTransaction()
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }
        public async Task<ProcessPaymentResult> ProcessPayment(PaymentTransaction paymentTransaction)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentTransactionStatus = TransactionStatus.Pending;
            return await Task.FromResult(result);
        }

        public Task PostProcessPayment(PaymentTransaction paymentTransaction)
        {
            //nothing
            return Task.CompletedTask;
        }
        /// <summary>
        /// Post redirect payment (used by payment gateways that redirecting to a another URL)
        /// </summary>
        /// <param name="paymentTransaction">Payment transaction</param>
        public Task PostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            //nothing
            return Task.CompletedTask;
        }
        public async Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            if (_CardOnDeliveryPaymentSettings.ShippableProductRequired && !cart.RequiresShipping())
                return true;

            return await Task.FromResult(false);
        }

        public async Task<double> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            if (_CardOnDeliveryPaymentSettings.AdditionalFee <= 0)
                return _CardOnDeliveryPaymentSettings.AdditionalFee;

            double result;
            if (_CardOnDeliveryPaymentSettings.AdditionalFeePercentage)
            {
                //percentage
                var orderTotalCalculationService = _serviceProvider.GetRequiredService<IOrderCalculationService>();
                var subtotal = await orderTotalCalculationService.GetShoppingCartSubTotal(cart, true);
                result = (double)((((float)subtotal.subTotalWithDiscount) * ((float)_CardOnDeliveryPaymentSettings.AdditionalFee)) / 100f);
            }
            else
            {
                //fixed value
                result = _CardOnDeliveryPaymentSettings.AdditionalFee;
            }

            if (result > 0)
            {
                var currencyService = _serviceProvider.GetRequiredService<ICurrencyService>();
                var workContext = _serviceProvider.GetRequiredService<IWorkContext>();
                result = await currencyService.ConvertFromPrimaryStoreCurrency(result, workContext.WorkingCurrency);
            }

            //return result;
            return result;
        }

        public async Task<IList<string>> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();
            return await Task.FromResult(warnings);
        }

        public async Task<PaymentTransaction> SavePaymentInfo(IFormCollection form)
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentCardOnDelivery";
        }

        public async Task<CapturePaymentResult> Capture(PaymentTransaction paymentTransaction)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return await Task.FromResult(result);
        }

        public async Task<bool> CanRePostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null)
                throw new ArgumentNullException(nameof(paymentTransaction));

            //it's not a redirection payment method. So we always return false
            return await Task.FromResult(false);
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

        /// <summary>
        /// Cancel a payment
        /// </summary>
        /// <returns>Result</returns>
        public async Task CancelPayment(PaymentTransaction paymentTransaction)
        {
            var paymentTransactionService = _serviceProvider.GetRequiredService<IPaymentTransactionService>();
            paymentTransaction.TransactionStatus = TransactionStatus.Canceled;
            await paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
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

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        public async Task<bool> SkipPaymentInfo()
        {
            return await Task.FromResult(_CardOnDeliveryPaymentSettings.SkipPaymentInfo);
        }

        public async Task<string> Description()
        {
            return await Task.FromResult(_translationService.GetResource("Plugins.Payment.CardOnDelivery.PaymentMethodDescription"));
        }

        public string LogoURL => "/Plugins/Payments.CardOnDelivery/logo.jpg";

    }
}
