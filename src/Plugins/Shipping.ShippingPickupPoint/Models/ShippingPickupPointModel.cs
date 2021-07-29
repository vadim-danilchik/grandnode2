using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Shipping.ShippingPickupPoint.Models
{
    public class ShippingPickupPointModel : BaseEntityModel
    {
        public ShippingPickupPointModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.ShippingPickupPointName")]
        public string ShippingPickupPointName { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.Description")]
        public string Description { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.OpeningHours")]
        public string OpeningHours { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.PickupFee")]
        public double PickupFee { get; set; }

        public List<SelectListItem> AvailableStores { get; set; }
        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.Store")]
        public string StoreId { get; set; }

        public string StoreName { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.City")]
        public string City { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.Address1")]
        public string Address1 { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        [GrandResourceDisplayName("Shipping.ShippingPickupPoint.Fields.Country")]
        public string CountryId { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
    }


}