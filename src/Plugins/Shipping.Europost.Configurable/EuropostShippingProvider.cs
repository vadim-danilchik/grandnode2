using Grand.Business.Catalog.Interfaces.Prices;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Interfaces.CheckoutAttributes;
using Grand.Business.Checkout.Interfaces.Shipping;
using Grand.Business.Checkout.Utilities;
using Grand.Business.Common.Extensions;
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
using Shipping.Europost.Configurable.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Shipping.Europost.Configurable
{
    public class EuropostShippingProvider : IShippingRateCalculationProvider
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITranslationService _translationService;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly EuropostShippingSettings _EuropostShippingSettings;
        private readonly IShippingEuropostService _shippingService;
        private readonly IUserFieldService _userFieldService;

        #endregion

        #region Ctor
        public EuropostShippingProvider(
            IWorkContext workContext,
            ITranslationService translationService,
            IProductService productService,
            IServiceProvider serviceProvider,
            IProductAttributeParser productAttributeParser,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            EuropostShippingSettings EuropostShippingSettings,
            IShippingEuropostService shippingService,
            IUserFieldService userFieldService
            )
        {
            _workContext = workContext;
            _translationService = translationService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _productAttributeParser = productAttributeParser;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _EuropostShippingSettings = EuropostShippingSettings;
            _shippingService = shippingService;
            _userFieldService = userFieldService;
        }
        #endregion

        #region Utilities

        private async Task<double?> GetRate(double subTotal, double weight)
        {

            var shippingService = _serviceProvider.GetRequiredService<IShippingEuropostService>();

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
            var maptranslate = _translationService.GetResource("Plugins.Shipping.Europost.Configurable.ShippingPoint.Map");
            shippingOption.Description = $"<a title=\"map\" href=\"{_EuropostShippingSettings.MapUrl}\" rel=\"noopener\" target=\"_blank\">{maptranslate}</a>";
            shippingOption.Name = _translationService.GetResource("Shipping.Europost.Configurable.FriendlyName");
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

        public string ConfigurationUrl => EuropostShippingDefaults.ConfigurationUrl;

        public string SystemName => EuropostShippingDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(EuropostShippingDefaults.FriendlyName);

        public int Priority => _EuropostShippingSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        public async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            var shippingMethodName = form["shippingoption"].ToString().Replace("___", "_").Split(new[] { '_' })[0];
            var shippingOptionId = form["selectedShippingOption"].ToString();

            if (string.IsNullOrEmpty(shippingOptionId))
                return new List<string>() { _translationService.GetResource("Plugins.Shipping.Europost.Configurable.SelectBeforeProceed") };

            if (shippingMethodName != _translationService.GetResource("Shipping.Europost.Configurable.FriendlyName"))
                throw new ArgumentException("shippingMethodName");

            var chosenShippingOption = await _shippingService.GetStoreShippingPointById(shippingOptionId);
            if (chosenShippingOption == null)
                return new List<string>() { _translationService.GetResource("Plugins.Shipping.Europost.Configurable.SelectBeforeProceed") };

            var forCustomer =
            string.Format("{0}<br>{1}<br>", chosenShippingOption.WarehouseName, chosenShippingOption.Info1);

            await _userFieldService.SaveField(
                _workContext.CurrentCustomer,
                SystemCustomerFieldNames.ShippingOptionAttributeDescription,
                forCustomer,
                    _workContext.CurrentStore.Id);

            var serializedObject = new Domain.EuropostShippingPointSerializable() {
                WarehouseName = chosenShippingOption.WarehouseName,
                Info1 = chosenShippingOption.Info1,
            };

            var stringBuilder = new StringBuilder();
            string serializedAttribute;
            using (var tw = new StringWriter(stringBuilder))
            {
                var xmlS = new XmlSerializer(typeof(Domain.EuropostShippingPointSerializable));
                xmlS.Serialize(tw, serializedObject);
                serializedAttribute = stringBuilder.ToString();
            }

            await _userFieldService.SaveField(_workContext.CurrentCustomer,
                SystemCustomerFieldNames.ShippingOptionAttribute,
                    serializedAttribute,
                    _workContext.CurrentStore.Id);

            return new List<string>();
        }

        public async Task<string> GetPublicViewComponentName()
        {
            return await Task.FromResult("EuropostShippingPoint");
        }

        #endregion
    }

}
