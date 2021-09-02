using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.Europost.Configurable.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipping.ShippingPoint.Components
{
    [ViewComponent(Name = "EuropostShippingPoint")]
    public class SelectedEuropostShippingPointViewComponent : ViewComponent
    {
        private readonly ITranslationService _translationService;
        private readonly IShippingEuropostService _shippingPointService;

        public SelectedEuropostShippingPointViewComponent(ITranslationService translationService, IShippingEuropostService shippingPointService)
        {
            _translationService = translationService;
            _shippingPointService = shippingPointService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string shippingOption)
        {
            var parameter = shippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (parameter == _translationService.GetResource("Shipping.Europost.Configurable.FriendlyName"))
            {
                var shippingPoints = await _shippingPointService.GetAllStoreShippingPoint();

                var shippingPointsModel = new List<SelectListItem>();
                foreach (var shippingPoint in shippingPoints)
                {
                    shippingPointsModel.Add(new SelectListItem() { Value = shippingPoint.Id, Text = shippingPoint.WarehouseName });
                }

                return View(shippingPointsModel);
            }
            return Content("ShippingPointController: given Shipping Point doesn't exist");

        }
    }
}
