using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shipping.ShippingPickupPoint.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipping.ShippingPickupPoint.Components
{
    [ViewComponent(Name = "ShippingPickupPoint")]
    public class SelectedShippingPickupPointViewComponent : ViewComponent
    {
        private readonly ITranslationService _translationService;
        private readonly IShippingPickupPointService _ShippingPickupPointService;
        private readonly IWorkContext _workContext;

        public SelectedShippingPickupPointViewComponent(ITranslationService translationService,
            IShippingPickupPointService ShippingPickupPointService, IWorkContext workContext)
        {
            _translationService = translationService;
            _ShippingPickupPointService = ShippingPickupPointService;
            _workContext = workContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(string shippingOption)
        {
            var parameter = shippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (parameter == _translationService.GetResource("Shipping.ShippingPickupPoint.PluginName"))
            {
                var ShippingPickupPoints = await _ShippingPickupPointService.GetAllStoreShippingPickupPoint(_workContext.CurrentStore.Id);

                var ShippingPickupPointsModel = new List<SelectListItem>();
                ShippingPickupPointsModel.Add(new SelectListItem() { Value = "", Text = _translationService.GetResource("Shipping.ShippingPickupPoint.SelectShippingOption") });

                foreach (var ShippingPickupPoint in ShippingPickupPoints)
                {
                    ShippingPickupPointsModel.Add(new SelectListItem() { Value = ShippingPickupPoint.Id, Text = ShippingPickupPoint.ShippingPickupPointName });
                }

                return View(ShippingPickupPointsModel);
            }
            return Content("ShippingPickupPointController: given Shipping Option doesn't exist");

        }
    }
}
