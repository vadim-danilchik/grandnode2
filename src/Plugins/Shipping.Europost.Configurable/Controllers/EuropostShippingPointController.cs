using Microsoft.AspNetCore.Mvc;
using Shipping.Europost.Configurable.Models;
using Shipping.Europost.Configurable.Services;
using System.Threading.Tasks;

namespace Shipping.Europost.Configurable.Controllers
{
    public class EuropostShippingPointController : Controller
    {
        private readonly IShippingEuropostService _shippingPointService;
        private readonly EuropostShippingSettings _europostShippingSettings;

        public EuropostShippingPointController(IShippingEuropostService shippingPointService, EuropostShippingSettings europostShippingSettings)
        {
            _shippingPointService = shippingPointService;
            _europostShippingSettings = europostShippingSettings;
        }
        public async Task<IActionResult> Get(string shippingOptionId)
        {
            var shippingPoint = await _shippingPointService.GetStoreShippingPointById(shippingOptionId);
            if (shippingPoint != null)
            {
                var viewModel = new EuropostShippingPointModel() {
                    Address1Id = shippingPoint.Address1Id,
                    Address7Id = shippingPoint.Address7Id,
                    Address7Name = shippingPoint.Address7Name,
                    Info1 = shippingPoint.Info1,
                    Latitude = shippingPoint.Latitude,
                    Longitude = shippingPoint.Longitude,
                    Note = shippingPoint.Note,
                    WarehouseId = shippingPoint.WarehouseId,
                    WarehouseName = shippingPoint.WarehouseName,
                    isNew = shippingPoint.isNew,
                    MapUrl = _europostShippingSettings.MapUrl
                };
                return View(viewModel);
            }
            return Content("ShippingPointController: given Shipping Option doesn't exist");
        }
    }
}
