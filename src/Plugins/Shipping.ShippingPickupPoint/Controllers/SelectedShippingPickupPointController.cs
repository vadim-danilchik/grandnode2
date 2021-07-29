using Grand.Business.Catalog.Interfaces.Prices;
using Grand.Business.Common.Interfaces.Directory;
using Microsoft.AspNetCore.Mvc;
using Shipping.ShippingPickupPoint.Models;
using Shipping.ShippingPickupPoint.Services;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint.Controllers
{
    public class SelectedShippingPickupPointController : Controller
    {
        private readonly IShippingPickupPointService _ShippingPickupPointService;
        private readonly ICountryService _countryService;
        private readonly IPriceFormatter _priceFormatter;

        public SelectedShippingPickupPointController(IShippingPickupPointService ShippingPickupPointService, ICountryService countryService, IPriceFormatter priceFormatter)
        {
            _ShippingPickupPointService = ShippingPickupPointService;
            _countryService = countryService;
            _priceFormatter = priceFormatter;
        }
        public async Task<IActionResult> Get(string shippingOptionId)
        {
            var ShippingPickupPoint = await _ShippingPickupPointService.GetStoreShippingPickupPointById(shippingOptionId);
            if (ShippingPickupPoint != null)
            {
                var viewModel = new PointModel() {
                    ShippingPickupPointName = ShippingPickupPoint.ShippingPickupPointName,
                    Description = ShippingPickupPoint.Description,
                    PickupFee = _priceFormatter.FormatShippingPrice(ShippingPickupPoint.PickupFee),
                    OpeningHours = ShippingPickupPoint.OpeningHours,
                    Address1 = ShippingPickupPoint.Address1,
                    City = ShippingPickupPoint.City,
                    CountryName = (await _countryService.GetCountryById(ShippingPickupPoint.CountryId))?.Name,
                    ZipPostalCode = ShippingPickupPoint.ZipPostalCode,
                };
                return View(viewModel);
            }
            return Content("ShippingPickupPointController: given Shipping Option doesn't exist");
        }
    }
}
