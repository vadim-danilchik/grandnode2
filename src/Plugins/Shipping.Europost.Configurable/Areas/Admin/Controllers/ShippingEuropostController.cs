using Grand.Business.Checkout.Interfaces.Shipping;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Stores;
using Grand.Business.Common.Services.Security;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.DataSource;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Grand.Domain.Directory;
using Microsoft.AspNetCore.Mvc;
using Shipping.Europost.Configurable.Domain;
using Shipping.Europost.Configurable.Models;
using Shipping.Europost.Configurable.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipping.Europost.Configurable.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.ShippingSettings)]
    public class ShippingEuropostController : BaseShippingController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IShippingMethodService _shippingMethodService;
        private readonly IStoreService _storeService;
        private readonly ICountryService _countryService;
        private readonly IShippingEuropostService _shippingService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly EuropostShippingSettings _europostShippingSettings;

        public ShippingEuropostController(
            IWarehouseService warehouseService,
            IShippingMethodService shippingMethodService,
            IStoreService storeService,
            ICountryService countryService,
            IShippingEuropostService shippingService,
            ISettingService settingService,
            ITranslationService translationService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            EuropostShippingSettings europostShippingSettings)
        {
            _warehouseService = warehouseService;
            _shippingMethodService = shippingMethodService;
            _storeService = storeService;
            _countryService = countryService;
            _shippingService = shippingService;
            _settingService = settingService;
            _translationService = translationService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _europostShippingSettings = europostShippingSettings;
        }
        public IActionResult Configure()
        {
            var model = new EuropostShippingSettingsModel 
            {
                UpdateRequestUrl = _europostShippingSettings.UpdateRequestUrl,
                UpdateRequestBody = _europostShippingSettings.UpdateRequestBody,
                MapUrl = _europostShippingSettings.MapUrl,
            };
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveGeneralSettings(EuropostShippingSettingsModel model)
        {
            //save settings
            _europostShippingSettings.UpdateRequestUrl = model.UpdateRequestUrl;
            _europostShippingSettings.UpdateRequestBody = model.UpdateRequestBody;
            _europostShippingSettings.MapUrl = model.MapUrl;

            await _settingService.SaveSetting(_europostShippingSettings);

            return Json(new { Result = true });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdatePoints()
        {
            await _shippingService.SyncShippingPoints();

            return Json(new { Result = true });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RatesByWeightList(DataSourceRequest command)
        {
            var records = await _shippingService.GetAllShippingByWeightRecords(command.Page - 1, command.PageSize);

            var sbwModel = new List<ShippingEuropostByWeightModel>();

            foreach (var x in records)
            {
                var m = new ShippingEuropostByWeightModel
                {
                    Id = x.Id,
                    WeightFrom = x.WeightFrom,
                    WeightTo = x.WeightTo,
                    WeightRate = x.WeightRate
                };

                sbwModel.Add(m);
            }
            var gridModel = new DataSourceResult
            {
                Data = sbwModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RateByWeightDelete(string id)
        {
            var sbw = await _shippingService.GetShippingByWeightRecordById(id);
            if (sbw != null)
                await _shippingService.DeleteShippingByWeightRecord(sbw);

            return new JsonResult("");
        }

        public async Task<IActionResult> AddByWeightPopup()
        {
            var model = new ShippingEuropostByWeightModel();
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)).Name;
            model.WeightTo = 1000000;

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddByWeightPopup(ShippingEuropostByWeightModel model)
        {
            var sbw = new ShippingEuropostByWeightRecord
            {
                WeightFrom = model.WeightFrom,
                WeightTo = model.WeightTo,
                WeightRate = model.WeightRate
            };
            await _shippingService.InsertShippingByWeightRecord(sbw);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //edit
        public async Task<IActionResult> EditByWeightPopup(string id)
        {
            var sbw = await _shippingService.GetShippingByWeightRecordById(id);
            if (sbw == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            var model = new ShippingEuropostByWeightModel
            {
                Id = sbw.Id,
                WeightFrom = sbw.WeightFrom,
                WeightTo = sbw.WeightTo,
                WeightRate = sbw.WeightRate,
                PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode,
                BaseWeightIn = (await _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)).Name
            };

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditByWeightPopup(ShippingEuropostByWeightModel model)
        {
            var sbw = await _shippingService.GetShippingByWeightRecordById(model.Id);
            if (sbw == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            sbw.WeightFrom = model.WeightFrom;
            sbw.WeightTo = model.WeightTo;
            sbw.WeightRate = model.WeightRate;

            await _shippingService.UpdateShippingByWeightRecord(sbw);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RatesByTotalList(DataSourceRequest command)
        {
            var records = await _shippingService.GetAllShippingByTotalRecords(command.Page - 1, command.PageSize);

            var sbtModel = new List<ShippingEuropostByTotalModel>();

            foreach (var x in records)
            {
                var m = new ShippingEuropostByTotalModel {
                    Id = x.Id,
                    TotalFrom = x.TotalFrom,
                    TotalTo = x.TotalTo,
                    TotalRate = x.TotalRate
                };

                sbtModel.Add(m);
            }
            var gridModel = new DataSourceResult {
                Data = sbtModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RateByTotalDelete(string id)
        {
            var sbt = await _shippingService.GetShippingByTotalRecordById(id);
            if (sbt != null)
                await _shippingService.DeleteShippingByTotalRecord(sbt);

            return new JsonResult("");
        }

        public async Task<IActionResult> AddByTotalPopup()
        {
            var model = new ShippingEuropostByTotalModel();
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.TotalTo = 1000000;

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> AddByTotalPopup(ShippingEuropostByTotalModel model)
        {
            var sbt = new ShippingEuropostByTotalRecord {
                TotalFrom = model.TotalFrom,
                TotalTo = model.TotalTo,
                TotalRate = model.TotalRate
            };
            await _shippingService.InsertShippingByTotalRecord(sbt);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //edit
        public async Task<IActionResult> EditByTotalPopup(string id)
        {
            var sbt = await _shippingService.GetShippingByTotalRecordById(id);
            if (sbt == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            var model = new ShippingEuropostByTotalModel {
                Id = sbt.Id,
                TotalFrom = sbt.TotalFrom,
                TotalTo = sbt.TotalTo,
                TotalRate = sbt.TotalRate,
                PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode
            };

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditByTotalPopup(ShippingEuropostByTotalModel model)
        {
            var sbt = await _shippingService.GetShippingByTotalRecordById(model.Id);
            if (sbt == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            sbt.TotalFrom = model.TotalFrom;
            sbt.TotalTo = model.TotalTo;
            sbt.TotalRate = model.TotalRate;

            await _shippingService.UpdateShippingByTotalRecord(sbt);

            ViewBag.RefreshPage = true;

            return View(model);
        }
    }
}
