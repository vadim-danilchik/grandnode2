﻿using Grand.Domain;

namespace Shipping.ShippingPickupPoint.Domain
{
    public class ShippingPickupPoints : BaseEntity
    {
        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string ShippingPickupPointName { get; set; }

        /// <summary>
        /// Gets or sets a description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets Address1
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets Zip postal code
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets CountryId
        /// </summary>
        public string CountryId { get; set; }

        /// <summary>
        /// Gets or sets a fee for the pickup
        /// </summary>
        public double PickupFee { get; set; }

        /// <summary>
        /// Gets or sets an oppening hours
        /// </summary>
        public string OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public string StoreId { get; set; }
    }
}