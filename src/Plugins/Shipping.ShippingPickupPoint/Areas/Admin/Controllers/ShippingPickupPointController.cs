using Grand.Business.Catalog.Interfaces.Prices;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Stores;
using Grand.Business.Common.Services.Security;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.DataSource;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.ShippingPickupPoint.Models;
using Shipping.ShippingPickupPoint.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.ShippingSettings)]
    public class ShippingPickupPointController : BaseShippingController
    {
        private readonly IWorkContext _workContext;
        private readonly IUserFieldService _userFieldService;
        private readonly ITranslationService _translationService;
        private readonly IShippingPickupPointService _ShippingPickupPointService;
        private readonly ICountryService _countryService;
        private readonly IStoreService _storeService;
        private readonly IPriceFormatter _priceFormatter;

        public ShippingPickupPointController(
            IWorkContext workContext,
            IUserFieldService userFieldService,
            ITranslationService translationService,
            IShippingPickupPointService ShippingPickupPointService,
            ICountryService countryService,
            IStoreService storeService,
            IPriceFormatter priceFormatter
            )
        {
            _workContext = workContext;
            _userFieldService = userFieldService;
            _translationService = translationService;
            _ShippingPickupPointService = ShippingPickupPointService;
            _countryService = countryService;
            _storeService = storeService;
            _priceFormatter = priceFormatter;
        }

        public IActionResult Configure()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> List(DataSourceRequest command)
        {
            var ShippingPickupPoints = await _ShippingPickupPointService.GetAllStoreShippingPickupPoint(storeId: "", pageIndex: command.Page - 1, pageSize: command.PageSize);
            var viewModel = new List<ShippingPickupPointModel>();

            foreach (var ShippingPickupPoint in ShippingPickupPoints)
            {
                var storeName = await _storeService.GetStoreById(ShippingPickupPoint.StoreId);
                viewModel.Add(new ShippingPickupPointModel
                {
                    ShippingPickupPointName = ShippingPickupPoint.ShippingPickupPointName,
                    Description = ShippingPickupPoint.Description,
                    Id = ShippingPickupPoint.Id,
                    OpeningHours = ShippingPickupPoint.OpeningHours,
                    PickupFee = ShippingPickupPoint.PickupFee,
                    StoreName = storeName != null ? storeName.Shortcut : _translationService.GetResource("Admin.Settings.StoreScope.AllStores"),

                });
            }

            return Json(new DataSourceResult
            {
                Data = viewModel,
                Total = ShippingPickupPoints.TotalCount
            });
        }

        private async Task<ShippingPickupPointModel> PrepareShippingPickupPointModel(ShippingPickupPointModel model)
        {
            model.AvailableCountries.Add(new SelectListItem { Text = _translationService.GetResource("Admin.Address.SelectCountry"), Value = string.Empty });
            foreach (var country in await _countryService.GetAllCountries(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = country.Name, Value = country.Id.ToString() });
            model.AvailableStores.Add(new SelectListItem { Text = _translationService.GetResource("Admin.Settings.StoreScope.AllStores"), Value = string.Empty });
            foreach (var store in await _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Shortcut, Value = store.Id.ToString() });
            return model;
        }

        public async Task<IActionResult> Create()
        {
            var model = new ShippingPickupPointModel();
            await PrepareShippingPickupPointModel(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingPickupPointModel model)
        {
            if (ModelState.IsValid)
            {
                var ShippingPickupPoint = model.ToEntity();
                await _ShippingPickupPointService.InsertStoreShippingPickupPoint(ShippingPickupPoint);

                ViewBag.RefreshPage = true;
            }

            await PrepareShippingPickupPointModel(model);

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var ShippingPickupPoints = await _ShippingPickupPointService.GetStoreShippingPickupPointById(id);
            var model = ShippingPickupPoints.ToModel();
            await PrepareShippingPickupPointModel(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShippingPickupPointModel model)
        {
            if (ModelState.IsValid)
            {
                var ShippingPickupPoint = await _ShippingPickupPointService.GetStoreShippingPickupPointById(model.Id);
                ShippingPickupPoint = model.ToEntity();
                await _ShippingPickupPointService.UpdateStoreShippingPickupPoint(ShippingPickupPoint);
            }
            ViewBag.RefreshPage = true;

            await PrepareShippingPickupPointModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _ShippingPickupPointService.GetStoreShippingPickupPointById(id);
            await _ShippingPickupPointService.DeleteStoreShippingPickupPoint(model);

            return new JsonResult("");
        }

    }
}
