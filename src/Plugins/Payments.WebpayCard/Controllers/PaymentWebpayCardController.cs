using Grand.Business.Checkout.Commands.Models.Orders;
using Grand.Business.Checkout.Extensions;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Queries.Models.Orders;
using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Logging;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Grand.SharedKernel;
using Grand.Web.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Payments.WebpayCard.Models;
using Payments.WebpayCard.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payments.WebpayCard.Controllers
{

    public class PaymentWebpayCardController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IWebpayHttpClient _webpayHttpClient;

        private readonly PaymentSettings _paymentSettings;
        private readonly WebpayCardPaymentSettings _WebpayCardPaymentSettings;

        public PaymentWebpayCardController(
            IWorkContext workContext,
            IPaymentService paymentService,
            IOrderService orderService,
            ILogger logger,
            IMediator mediator,
            IPaymentTransactionService paymentTransactionService,
            IWebpayHttpClient webpayHttpClient,
            WebpayCardPaymentSettings WebpayCardPaymentSettings,
            PaymentSettings paymentSettings)
        {
            _workContext = workContext;
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
            _mediator = mediator;
            _paymentTransactionService = paymentTransactionService;
            _webpayHttpClient = webpayHttpClient;
            _WebpayCardPaymentSettings = WebpayCardPaymentSettings;
            _paymentSettings = paymentSettings;
        }


        private string QueryString(string name)
        {
            if (StringValues.IsNullOrEmpty(HttpContext.Request.Query[name]))
                return default;

            return HttpContext.Request.Query[name].ToString();
        }

        public async Task<IActionResult> SuccessPayment()
        {
            var orderNumber = QueryString("wsb_order_num");

            if (_paymentService.LoadPaymentMethodBySystemName("Payments.WebpayCard") is not WebpayCardPaymentProvider processor ||
                !processor.IsPaymentMethodActive(_paymentSettings))
                throw new GrandException("Webpay Card module cannot be loaded");

            if (orderNumber != null)
            {
                Order order = await _orderService.GetOrderByNumber(int.Parse(orderNumber));
                if (order != null)
                {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
            }
            
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> NotifyPayment(PaymentNotifyModel model)
        {
            if (_paymentService.LoadPaymentMethodBySystemName("Payments.WebpayCard") is not WebpayCardPaymentProvider processor ||
                !processor.IsPaymentMethodActive(_paymentSettings))
                throw new GrandException("Webpay Card module cannot be loaded");

            var signature = string.Concat(
                model.batch_timestamp,
                model.currency_id,
                model.amount,
                model.payment_method,
                model.order_id,
                model.site_order_id,
                model.transaction_id,
                model.payment_type,
                model.rrn,
                _WebpayCardPaymentSettings.SecretKey);

            var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(signature));
            var hash = string.Empty;
            foreach (var b in md5)
            {
                hash += b.ToString("x2");
            }

            if (hash == model.wsb_signature)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Webpay:");
                sb.AppendLine(nameof(model.action) + ": " + model.action);
                sb.AppendLine(nameof(model.amount) + ": " + model.amount);
                sb.AppendLine(nameof(model.approval) + ": " + model.approval);
                sb.AppendLine(nameof(model.batch_timestamp) + ": " + model.batch_timestamp);
                sb.AppendLine(nameof(model.currency_id) + ": " + model.currency_id);
                sb.AppendLine(nameof(model.order_id) + ": " + model.order_id);
                sb.AppendLine(nameof(model.payment_method) + ": " + model.payment_method);
                sb.AppendLine(nameof(model.payment_type) + ": " + model.payment_type);
                sb.AppendLine(nameof(model.rc) + ": " + model.rc);
                sb.AppendLine(nameof(model.rrn) + ": " + model.rrn);
                sb.AppendLine(nameof(model.site_order_id) + ": " + model.site_order_id);
                sb.AppendLine(nameof(model.transaction_id) + ": " + model.transaction_id);
                sb.AppendLine(nameof(model.wsb_signature) + ": " + model.wsb_signature);

                var newPaymentStatus = WebpayHelper.GetPaymentStatus(model.payment_type);
                sb.AppendLine("Статус платежа: " + newPaymentStatus);

                var order = await _orderService.GetOrderByNumber(int.Parse(model.site_order_id));
                if (order != null)
                {
                    //order note
                    await _orderService.InsertOrderNote(new OrderNote {
                        Note = sb.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                    });
                    var paymentTransaction = await _paymentTransactionService.GetByOrdeGuid(order.OrderGuid);

                    switch (newPaymentStatus)
                    {
                        case PaymentStatus.Pending:
                            {
                            }
                            break;
                        case PaymentStatus.Paid:
                            {
                                //validate order total
                                if (Math.Round(double.Parse(model.amount, NumberStyles.Number, CultureInfo.InvariantCulture), 2).Equals(Math.Round(order.OrderTotal, 2)))
                                {
                                    //valid
                                    if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
                                    {
                                        paymentTransaction.AuthorizationTransactionId = model.transaction_id;
                                        await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);

                                        await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
                                    }
                                }
                                else
                                {
                                    //not valid
                                    string errorStr = string.Format("Webpay. Returned order total {0} doesn't equal order total {1}. Order# {2}.", model.amount, order.OrderTotal * order.CurrencyRate, order.Id);
                                    //log
                                    _logger.Error(errorStr);
                                    //order note
                                    await _orderService.InsertOrderNote(new OrderNote {
                                        Note = errorStr,
                                        DisplayToCustomer = false,
                                        CreatedOnUtc = DateTime.UtcNow,
                                        OrderId = order.Id,
                                    });
                                }
                            }
                            break;
                        case PaymentStatus.Refunded:
                            {
                                var totalToRefund = Math.Abs(double.Parse(model.amount, NumberStyles.Number));
                                if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                {
                                    //refund
                                    if (await _mediator.Send(new CanRefundOfflineQuery() { PaymentTransaction = paymentTransaction }))
                                    {
                                        await _mediator.Send(new RefundOfflineCommand() { PaymentTransaction = paymentTransaction });
                                    }
                                }
                                else
                                {
                                    //partial refund
                                    if (await _mediator.Send(new CanPartiallyRefundOfflineQuery() { PaymentTransaction = paymentTransaction, AmountToRefund = totalToRefund }))
                                    {
                                        await _mediator.Send(new PartiallyRefundOfflineCommand() { PaymentTransaction = paymentTransaction, AmountToRefund = totalToRefund });
                                    }
                                }
                            }
                            break;
                        case PaymentStatus.Voided:
                            {
                                if (await _mediator.Send(new CanVoidOfflineQuery() { PaymentTransaction = paymentTransaction }))
                                {
                                    await _mediator.Send(new VoidOfflineCommand() { PaymentTransaction = paymentTransaction });
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _logger.Error("Webpay IPN. Order is not found", new GrandException(sb.ToString()));
                }

                return Ok();
            }

            return BadRequest();
        }

        public async Task<IActionResult> CancelPayment()
        {
            var order = (await _orderService.SearchOrders(storeId: _workContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)).FirstOrDefault();
            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("HomePage");
        }
    }
}