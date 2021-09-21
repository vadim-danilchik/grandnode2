using Grand.Business.Catalog.Interfaces.Prices;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Interfaces.CheckoutAttributes;
using Grand.Business.Checkout.Interfaces.Shipping;
using Grand.Business.Checkout.Utilities;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Shipping;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shipping.Courier.Configurable.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipping.Courier.Configurable
{
    public class CourierShippingProvider : IShippingRateCalculationProvider
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITranslationService _translationService;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly CourierShippingSettings _courierShippingSettings;

        #endregion

        #region Ctor
        public CourierShippingProvider(
            IWorkContext workContext,
            ITranslationService translationService,
            IProductService productService,
            IServiceProvider serviceProvider,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            CourierShippingSettings courierShippingSettings)
        {
            _workContext = workContext;
            _translationService = translationService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _productAttributeParser = productAttributeParser;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _courierShippingSettings = courierShippingSettings;
        }
        #endregion

        #region Utilities

        private async Task<double?> GetRate(double subTotal, double weight)
        {

            var shippingService = _serviceProvider.GetRequiredService<IShippingCourierService>();

            var shippingByWeightRecord = await shippingService.FindShippingByWeightRecord(weight);
            var shippingByToalRecord = await shippingService.FindShippingByTotalRecord(subTotal);

            var shippingByWeight = shippingByWeightRecord?.WeightRate;
            var shippingByTotal = shippingByToalRecord?.TotalRate;

            double shippingTotal = 0;
            if (shippingByWeight.HasValue && shippingByTotal.HasValue && shippingByWeight < shippingByTotal)
            {
                shippingTotal = (double)shippingByWeight;
            }
            if (shippingByWeight.HasValue && shippingByTotal.HasValue && shippingByWeight > shippingByTotal)
            {
                shippingTotal = (double)shippingByTotal;
            }
            if (shippingByWeight.HasValue && !shippingByTotal.HasValue)
            {
                shippingTotal = (double)shippingByWeight;
            }
            if (!shippingByWeight.HasValue && shippingByTotal.HasValue)
            {
                shippingTotal = (double)shippingByTotal;
            }

            if (shippingTotal < 0)
            {
                shippingTotal = 0;
            }
            return shippingTotal;
        }

        #endregion

        #region Methods

        private async Task<double> GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            var product = await _productService.GetProductById(shoppingCartItem.ProductId);
            if (product == null)
                return 0;

            //attribute weight
            double attributesTotalWeight = 0;
            if (shoppingCartItem.Attributes != null && shoppingCartItem.Attributes.Any())
            {
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(product, shoppingCartItem.Attributes);
                foreach (var attributeValue in attributeValues)
                {
                    switch (attributeValue.AttributeValueTypeId)
                    {
                        case AttributeValueType.Simple:
                            {
                                //simple attribute
                                attributesTotalWeight += attributeValue.WeightAdjustment;
                            }
                            break;
                        case AttributeValueType.AssociatedToProduct:
                            {
                                //bundled product
                                var associatedProduct = await _productService.GetProductById(attributeValue.AssociatedProductId);
                                if (associatedProduct != null && associatedProduct.IsShipEnabled)
                                {
                                    attributesTotalWeight += associatedProduct.Weight * attributeValue.Quantity;
                                }
                            }
                            break;
                    }
                }
            }
            var weight = product.Weight + attributesTotalWeight;
            return weight;
        }

        private async Task<double> GetTotalWeight(GetShippingOptionRequest request, bool includeCheckoutAttributes = true)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Customer customer = request.Customer;

            double totalWeight = 0;
            //shopping cart items
            foreach (var packageItem in request.Items)
                totalWeight += await GetShoppingCartItemWeight(packageItem.ShoppingCartItem) * packageItem.GetQuantity();

            //checkout attributes
            if (customer != null && includeCheckoutAttributes)
            {
                var checkoutAttributes = customer.GetUserFieldFromEntity<List<CustomAttribute>>(SystemCustomerFieldNames.CheckoutAttributes, request.StoreId);
                if (checkoutAttributes.Any())
                {
                    var attributeValues = await _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributes);
                    foreach (var attributeValue in attributeValues)
                        totalWeight += attributeValue.WeightAdjustment;
                }
            }
            return totalWeight;
        }

        public async Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || getShippingOptionRequest.Items.Count == 0)
            {
                response.AddError("No shipment items");
                return response;
            }
            //if (getShippingOptionRequest.ShippingAddress == null)
            //{
            //    response.AddError("Shipping address is not set");
            //    return response;
            //}

            double subTotal = 0;
            var priceCalculationService = _serviceProvider.GetRequiredService<IPricingService>();

            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                if (packageItem.ShoppingCartItem.IsFreeShipping)
                    continue;

                var product = await _productService.GetProductById(packageItem.ShoppingCartItem.ProductId);
                if (product != null)
                    subTotal += (await priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem, product)).subTotal;
            }

            double weight = await GetTotalWeight(getShippingOptionRequest);

            var rate = await GetRate(subTotal, weight);

            var shippingOption = new ShippingOption();
            shippingOption.Name = _translationService.GetResource("Shipping.Courier.Configurable.FriendlyName");
            shippingOption.Rate = await _currencyService.ConvertFromPrimaryStoreCurrency(rate.Value, _workContext.WorkingCurrency);
            response.ShippingOptions.Add(shippingOption);

            return response;
        }

        public async Task<double?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return await Task.FromResult(default(double?));
        }

        public async Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            return await Task.FromResult(false);
        }

        #endregion

        #region Properties
        public IShipmentTracker ShipmentTracker => null;

        public ShippingRateCalculationType ShippingRateCalculationType => ShippingRateCalculationType.Off;

        public string ConfigurationUrl => CourierShippingDefaults.ConfigurationUrl;

        public string SystemName => CourierShippingDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(CourierShippingDefaults.FriendlyName);

        public int Priority => _courierShippingSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        public async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            //you can implement here any validation logic
            return await Task.FromResult(new List<string>());
        }

        public async Task<string> GetPublicViewComponentName()
        {
            return await Task.FromResult("");
        }

        #endregion
    }

}
