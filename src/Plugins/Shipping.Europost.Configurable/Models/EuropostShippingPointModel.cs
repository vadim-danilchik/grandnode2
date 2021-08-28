using Grand.Infrastructure.Models;

namespace Shipping.Europost.Configurable.Models
{
    public class EuropostShippingPointModel : BaseEntityModel
    {
        public string Address1Id { get; set; }

        public string Address7Id { get; set; }

        public string Address7Name { get; set; }

        public string Info1 { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Note { get; set; }

        public string WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public string isNew { get; set; }

        public string MapUrl { get; set; }
    }


}