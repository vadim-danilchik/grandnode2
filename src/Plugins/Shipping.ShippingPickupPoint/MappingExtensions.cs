using Grand.Infrastructure.Mapper;
using Shipping.ShippingPickupPoint.Domain;
using Shipping.ShippingPickupPoint.Models;

namespace Shipping.ShippingPickupPoint
{
    public static class MappingExtensions
    {
        public static ShippingPickupPointModel ToModel(this ShippingPickupPoints entity)
        {
            return entity.MapTo<ShippingPickupPoints, ShippingPickupPointModel>();
        }

        public static ShippingPickupPoints ToEntity(this ShippingPickupPointModel model)
        {
            return model.MapTo<ShippingPickupPointModel, ShippingPickupPoints>();
        }

        public static ShippingPickupPoints ToEntity(this ShippingPickupPointModel model, ShippingPickupPoints destination)
        {
            return model.MapTo(destination);
        }

    }
}
