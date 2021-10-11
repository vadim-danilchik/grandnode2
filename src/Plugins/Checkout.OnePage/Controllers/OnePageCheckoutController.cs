﻿using Checkout.OnePage.Models;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Checkout.Commands.Models.Orders;
using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Extensions;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Interfaces.Shipping;
using Grand.Business.Checkout.Services.Orders;
using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Addresses;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Logging;
using Grand.Business.Customers.Interfaces;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Domain.Shipping;
using Grand.Infrastructure;
using Grand.Infrastructure.Extensions;
using Grand.Web.Common.Controllers;
using Grand.Web.Controllers;
using Grand.Web.Extensions;
using Grand.Web.Features.Models.Checkout;
using Grand.Web.Features.Models.Common;
using Grand.Web.Models.Checkout;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkout.OnePage.Controllers
{
    public class OnePageCheckoutController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly ITranslationService _translationService;
        private readonly ICustomerService _customerService;
        private readonly IGroupService _groupService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserFieldService _userFieldService;
        private readonly IShippingService _shippingService;
        private readonly IPickupPointService _pickupPointService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IMediator _mediator;
        private readonly IProductService _productService;
        private readonly IShoppingCartValidator _shoppingCartValidator;
        private readonly OrderSettings _orderSettings;
        private readonly LoyaltyPointsSettings _loyaltyPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;

        public OnePageCheckoutController(
            IWorkContext workContext,
            ITranslationService translationService,
            ICustomerService customerService,
            IGroupService groupService,
            IShoppingCartService shoppingCartService,
            IUserFieldService userFieldService,
            IShippingService shippingService,
            IPickupPointService pickupPointService,
            IPaymentService paymentService,
            IPaymentTransactionService paymentTransactionService,
            ILogger logger,
            IOrderService orderService,
            IAddressAttributeParser addressAttributeParser,
            ICustomerActivityService customerActivityService,
            IMediator mediator,
            IProductService productService,
            IShoppingCartValidator shoppingCartValidator,
            OrderSettings orderSettings,
            LoyaltyPointsSettings loyaltyPointsSettings,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings)
        {
            _workContext = workContext;
            _translationService = translationService;
            _customerService = customerService;
            _groupService = groupService;
            _shoppingCartService = shoppingCartService;
            _userFieldService = userFieldService;
            _shippingService = shippingService;
            _pickupPointService = pickupPointService;
            _paymentService = paymentService;
            _paymentTransactionService = paymentTransactionService;
            _logger = logger;
            _orderService = orderService;
            _addressAttributeParser = addressAttributeParser;
            _customerActivityService = customerActivityService;
            _mediator = mediator;
            _productService = productService;
            _shoppingCartValidator = shoppingCartValidator;
            _orderSettings = orderSettings;
            _loyaltyPointsSettings = loyaltyPointsSettings;
            _paymentSettings = paymentSettings;
            _shippingSettings = shippingSettings;
            _addressSettings = addressSettings;
        }

        public async Task<IActionResult> OnePageCheckoutStart()
        {
            //validation
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, ShoppingCartType.ShoppingCart, ShoppingCartType.Auctions);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if ((await _groupService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed))
                return Challenge();

            //validation (each shopping cart item)
            foreach (ShoppingCartItem sci in cart)
            {
                var product = await _productService.GetProductById(sci.ProductId);
                var sciWarnings = await _shoppingCartValidator.GetShoppingCartItemWarnings(_workContext.CurrentCustomer, sci, product, new ShoppingCartValidatorOptions());
                if (sciWarnings.Any())
                    return RedirectToRoute("ShoppingCart", new { checkoutAttributes = true });
            }

            var requiresShipping = cart.RequiresShipping();

            var model = new OnePageCheckoutModel {
                ShippingRequired = requiresShipping,
                ShippingAddress = await _mediator.Send(new GetShippingAddress() {
                    Currency = _workContext.WorkingCurrency,
                    Customer = _workContext.CurrentCustomer,
                    Language = _workContext.WorkingLanguage,
                    Store = _workContext.CurrentStore,
                    PrePopulateNewAddressWithCustomerFields = true
                }),
                ShippingMethod = await _mediator.Send(new GetShippingMethod() {
                    Cart = cart,
                    Currency = _workContext.WorkingCurrency,
                    Customer = _workContext.CurrentCustomer,
                    Language = _workContext.WorkingLanguage,
                    ShippingAddress = _workContext.CurrentCustomer.ShippingAddress,
                    Store = _workContext.CurrentStore
                })
            };

            //-------------------------------------------------------------------------
            bool isPaymentWorkflowRequired = await _mediator.Send(new GetIsPaymentWorkflowRequired() { Cart = cart, UseLoyaltyPoints = false });
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                string filterByCountryId = "";
                if (_addressSettings.CountryEnabled &&
                    _workContext.CurrentCustomer.BillingAddress != null &&
                    !string.IsNullOrEmpty(_workContext.CurrentCustomer.BillingAddress.CountryId))
                {
                    filterByCountryId = _workContext.CurrentCustomer.BillingAddress.CountryId;
                }

                //payment is required
                model.PaymentMethod = await _mediator.Send(new GetPaymentMethod() {
                    Cart = cart,
                    Currency = _workContext.WorkingCurrency,
                    Customer = _workContext.CurrentCustomer,
                    FilterByCountryId = filterByCountryId,
                    Language = _workContext.WorkingLanguage,
                    Store = _workContext.CurrentStore
                });

            }

            return View(model);
        }

        public async Task<IActionResult> SavePickUpInSore(IFormCollection form)
        {
            try
            {
                //validation
                var customer = _workContext.CurrentCustomer;
                var store = _workContext.CurrentStore;

                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, ShoppingCartType.ShoppingCart, ShoppingCartType.Auctions);
                await CartValidate(cart);

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                if (_shippingSettings.AllowPickUpInStore)
                {
                        //customer decided to pick up in store
                        //no shipping address selected
                        _workContext.CurrentCustomer.ShippingAddress = null;
                        await _customerService.RemoveShippingAddress(_workContext.CurrentCustomer.Id);

                        //clear shipping option XML/Description
                        await _userFieldService.SaveField(_workContext.CurrentCustomer, SystemCustomerFieldNames.ShippingOptionAttribute, "", _workContext.CurrentStore.Id);
                        await _userFieldService.SaveField(_workContext.CurrentCustomer, SystemCustomerFieldNames.ShippingOptionAttributeDescription, "", _workContext.CurrentStore.Id);

                        var pickupPoint = form["pickup-point-id"];
                        var pickupPoints = await _pickupPointService.LoadActivePickupPoints(_workContext.CurrentStore.Id);
                        var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint));
                        if (selectedPoint == null)
                            throw new Exception("Pickup point is not allowed");

                        //save "pick up in store" shipping method
                        var pickUpInStoreShippingOption = new ShippingOption {
                            Name = string.Format(_translationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateProviderSystemName = string.Format("PickupPoint_{0}", selectedPoint.Id)
                        };

                        await _userFieldService.SaveField(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.SelectedShippingOption,
                        pickUpInStoreShippingOption,
                        _workContext.CurrentStore.Id);

                        await _userFieldService.SaveField(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.SelectedPickupPoint,
                        selectedPoint.Id,
                        _workContext.CurrentStore.Id);
                }
                                
                if (ModelState.IsValid)
                {
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel {
                            name = "shipping-method"
                        },
                        goto_section = "shipping_method"
                    });
                }

                var message = "Something went wrong";
                return Json(new { error = 1, message = message });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public async Task<IActionResult> SaveShippingMethod(IFormCollection form)
        {
            try
            {
                //validation
                var customer = _workContext.CurrentCustomer;
                var store = _workContext.CurrentStore;

                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, ShoppingCartType.ShoppingCart, ShoppingCartType.Auctions);
                await CartValidate(cart);

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");
                                
                //parse selected method 
                string shippingoption = form["shippingoption"];
                if (String.IsNullOrEmpty(shippingoption))
                    throw new Exception("Selected shipping method can't be parsed");
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");
                string selectedName = splittedOption[0];
                string shippingRateProviderSystemName = splittedOption[1];

                //clear shipping option XML/Description
                await _userFieldService.SaveField(customer, SystemCustomerFieldNames.ShippingOptionAttribute, "", store.Id);
                await _userFieldService.SaveField(customer, SystemCustomerFieldNames.ShippingOptionAttributeDescription, "", store.Id);

                //validate customer's input
                var warnings = (await ValidateShippingForm(form)).ToList();

                //find it
                //performance optimization. try cache first
                var shippingOptions = await customer.GetUserField<List<ShippingOption>>(_userFieldService, SystemCustomerFieldNames.OfferedShippingOptions, store.Id);
                if (shippingOptions == null || shippingOptions.Count == 0)
                {
                    //not found? load them using shipping service
                    shippingOptions = (await _shippingService
                        .GetShippingOptions(customer, cart, customer.ShippingAddress, shippingRateProviderSystemName, store))
                        .ShippingOptions
                        .ToList();
                }
                else
                {
                    //loaded cached results. filter result by a chosen Shipping rate  method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateProviderSystemName.Equals(shippingRateProviderSystemName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));
                if (shippingOption == null)
                    throw new Exception("Selected shipping method can't be loaded");

                //save
                await _userFieldService.SaveField(customer, SystemCustomerFieldNames.SelectedShippingOption, shippingOption, store.Id);

                //set value indicating that "pick up in store" option has not been chosen
                await _userFieldService.SaveField(_workContext.CurrentCustomer, SystemCustomerFieldNames.SelectedPickupPoint, "", _workContext.CurrentStore.Id);

                if (ModelState.IsValid)
                {
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel {
                            name = "shipping-method"
                        },
                        goto_section = "shipping_method"
                    });
                }

                var message = String.Join(", ", warnings.ToArray());
                return Json(new { error = 1, message = message });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public async Task<IActionResult> ConfirmOrder(CheckoutShippingAddressModel model, IFormCollection form)
        {
            try
            {
                #region ShippingAddress
                //validation
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, ShoppingCartType.ShoppingCart, ShoppingCartType.Auctions);
                await CartValidate(cart);

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                //Pick up in store?
                var pickupInstoreField = await _userFieldService.GetFieldsForEntity<string>(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.SelectedPickupPoint,
                        _workContext.CurrentStore.Id);

                var pickupInstore = pickupInstoreField != null;

                string shippingAddressId = form["shipping_address_id"];

                if (!pickupInstore)
                {
                    if (!String.IsNullOrEmpty(shippingAddressId))
                    {
                        //existing address
                        var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                        if (address == null)
                            throw new Exception("Address can't be loaded");

                        _workContext.CurrentCustomer.ShippingAddress = address;
                        await _customerService.UpdateShippingAddress(address, _workContext.CurrentCustomer.Id);
                    }
                    else
                    {
                        ModelState.Clear();
                        //new address
                        await TryUpdateModelAsync(model.NewAddress, "ShippingNewAddress");
                        //custom address attributes
                        var customAttributes = await _mediator.Send(new GetParseCustomAddressAttributes() { Form = form });
                        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarnings(customAttributes);
                        foreach (var error in customAttributeWarnings)
                        {
                            ModelState.AddModelError("", error);
                        }

                        //validate model
                        TryValidateModel(model.NewAddress);
                        if (!ModelState.IsValid)
                        {
                            var errors = ModelState.Values.SelectMany(v => v.Errors);
                            foreach (var item in errors)
                            {
                                string tt = item.ErrorMessage;
                            }
                            //model is not valid. redisplay the form with errors
                            var shippingAddressModel = await _mediator.Send(new GetShippingAddress() {
                                Currency = _workContext.WorkingCurrency,
                                Customer = _workContext.CurrentCustomer,
                                Language = _workContext.WorkingLanguage,
                                Store = _workContext.CurrentStore,
                                SelectedCountryId = model.NewAddress.CountryId,
                                OverrideAttributes = customAttributes,
                            });

                            shippingAddressModel.NewAddressPreselected = true;
                            return Json(new
                            {
                                update_section = new UpdateSectionJsonModel {
                                    name = "shipping",
                                    model = shippingAddressModel
                                },
                                wrong_shipping_address = true,
                                model_state = SerializeModelState(ModelState)
                            });
                        }

                        //try to find an address with the same values (don't duplicate records)
                        var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                            model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                            model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                            model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                            model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                            model.NewAddress.CountryId);
                        if (address == null)
                        {
                            address = model.NewAddress.ToEntity();
                            address.Attributes = customAttributes;
                            address.CreatedOnUtc = DateTime.UtcNow;
                            address.AddressType = _addressSettings.AddressTypeEnabled ? (model.BillToTheSameAddress ? AddressType.Any : AddressType.Shipping) : AddressType.Any;
                            //other null validations
                            _workContext.CurrentCustomer.Addresses.Add(address);
                            await _customerService.InsertAddress(address, _workContext.CurrentCustomer.Id);
                        }
                        _workContext.CurrentCustomer.ShippingAddress = address;
                        await _customerService.UpdateShippingAddress(address, _workContext.CurrentCustomer.Id);
                    }
                }


                if (model.BillToTheSameAddress && !pickupInstore)
                {
                    _workContext.CurrentCustomer.BillingAddress = _workContext.CurrentCustomer.ShippingAddress;
                    await _customerService.UpdateBillingAddress(_workContext.CurrentCustomer.BillingAddress, _workContext.CurrentCustomer.Id);
                    await _userFieldService.SaveField<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerFieldNames.SelectedShippingOption, null, _workContext.CurrentStore.Id);
                    //return await LoadStepAfterBillingAddress(cart);
                }
                #endregion

                #region PaymentMethod
                string paymentmethod = form["paymentmethod"];
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var paymentModel = new CheckoutPaymentMethodModel();
                await TryUpdateModelAsync(paymentModel);

                //loyalty points
                if (_loyaltyPointsSettings.Enabled)
                {
                    await _userFieldService.SaveField(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.UseLoyaltyPointsDuringCheckout, paymentModel.UseLoyaltyPoints,
                        _workContext.CurrentStore.Id);
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !paymentMethodInst.IsAuthenticateStore(_workContext.CurrentStore))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                await _userFieldService.SaveField(_workContext.CurrentCustomer,
                    SystemCustomerFieldNames.SelectedPaymentMethod, paymentmethod, _workContext.CurrentStore.Id);

                var paymentTransaction = await paymentMethodInst.InitPaymentTransaction();
                if (paymentTransaction != null)
                {
                    await _userFieldService.SaveField(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.PaymentTransaction, paymentTransaction.Id, _workContext.CurrentStore.Id);
                }
                else
                    await _userFieldService.SaveField<string>(_workContext.CurrentCustomer,
                        SystemCustomerFieldNames.PaymentTransaction, null, _workContext.CurrentStore.Id);

                #endregion

                #region Confirm
                //prevent 2 orders being placed within an X seconds time frame
                if (!await _mediator.Send(new GetMinOrderPlaceIntervalValid() {
                    Customer = _workContext.CurrentCustomer,
                    Store = _workContext.CurrentStore
                }))
                    throw new Exception(_translationService.GetResource("Checkout.MinOrderPlacementInterval"));


                var placeOrderResult = await _mediator.Send(new PlaceOrderCommand());
                if (placeOrderResult.Success)
                {
                    await _customerActivityService.InsertActivity("PublicStore.PlaceOrder", "", _translationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), placeOrderResult.PlacedOrder.Id);

                    var confirmPaymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PaymentTransaction.PaymentMethodSystemName);
                    if (confirmPaymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 });

                    if (confirmPaymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        var storeLocation = _workContext.CurrentStore.SslEnabled ? _workContext.CurrentStore.SecureUrl.TrimEnd('/') : _workContext.CurrentStore.Url.TrimEnd('/');
                        //redirect
                        return Json(new
                        {
                            redirect = $"{storeLocation}/checkout/CompleteRedirectionPayment?paymentTransactionId={placeOrderResult.PaymentTransaction.Id}",
                        });
                    }

                    await _paymentService.PostProcessPayment(placeOrderResult.PaymentTransaction);
                    //success
                    return Json(new { success = 1 });
                }
                else
                {
                    //error
                    var confirmOrderModel = new CheckoutConfirmModel();
                    foreach (var error in placeOrderResult.Errors)
                        confirmOrderModel.Warnings.Add(error);

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel {
                            name = "confirm-order",
                            model = confirmOrderModel
                        },
                        goto_section = "confirm_order"
                    });
                }
                #endregion
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public async Task<IActionResult> OnePageCheckoutCompleted(string orderId)
        {
            //validation
            if ((await _groupService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed))
                return Challenge();

            Order order = null;
            if (!String.IsNullOrEmpty(orderId))
            {
                order = await _orderService.GetOrderById(orderId);
            }
            if (order == null)
            {
                order = (await _orderService.SearchOrders(storeId: _workContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return RedirectToRoute("HomePage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = new CheckoutCompletedModel {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderCode = order.Code,
            };

            return View(model);
        }

        public async Task<IActionResult> CompleteRedirectionPayment(string paymentTransactionId)
        {
            try
            {
                if ((await _groupService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed))
                    return Challenge();


                var paymentTransaction = await _paymentTransactionService.GetById(paymentTransactionId);
                if (paymentTransaction == null)
                    return RedirectToRoute("HomePage");

                //get the order
                var order = await _orderService.GetOrderByGuid(paymentTransaction.OrderGuid);
                if (order == null)
                    return RedirectToRoute("HomePage");

                if (paymentTransaction.OrderGuid != order.OrderGuid)
                    return RedirectToRoute("HomePage");

                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentTransaction.PaymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("HomePage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("HomePage");

                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 5)
                    return RedirectToRoute("HomePage");

                await _paymentService.PostRedirectPayment(paymentTransaction);

                if (IsRequestBeingRedirected || IsPostBeingDone)
                {
                    return Content("Redirected");
                }
                return RedirectToRoute("OnePageCheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Content(exc.Message);
            }
        }

        private async Task CartValidate(IList<ShoppingCartItem> cart)
        {
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            if (await _groupService.IsGuest(_workContext.CurrentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                throw new Exception("Anonymous checkout is not allowed");
        }

        #region Utilities

        [NonAction]
        protected IShippingRateCalculationProvider GetShippingComputation(string input)
        {
            var shippingMethodName = input.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries)[1];
            var shippingMethod = _shippingService.LoadShippingRateCalculationProviderBySystemName(shippingMethodName);
            if (shippingMethod == null)
                throw new Exception("Shipping method is not selected");

            return shippingMethod;
        }

        [NonAction]
        protected async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            var warnings = (await GetShippingComputation(form["shippingoption"]).ValidateShippingForm(form)).ToList();
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            return warnings;
        }

        protected IList<string> SerializeModelState(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            var valuerrors = modelState.Where(entry => entry.Value.Errors.Any());
            foreach (var item in valuerrors)
            {
                foreach (var er in item.Value.Errors)
                {
                    errors.Add(er.ErrorMessage);
                }
            }
            return errors;
        }

        protected virtual bool IsRequestBeingRedirected {
            get {
                var response = HttpContext.Response;
                return new List<int> { 301, 302 }.Contains(response.StatusCode);
            }
        }
        protected virtual bool IsPostBeingDone {
            get {
                if (HttpContext.Items["grand.IsPOSTBeingDone"] == null)
                    return false;
                return Convert.ToBoolean(HttpContext.Items["grand.IsPOSTBeingDone"]);
            }
            set {
                HttpContext.Items["grand.IsPOSTBeingDone"] = value;
            }
        }

        #endregion
    }
}
