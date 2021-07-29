using AutoMapper;
using Grand.Infrastructure.Mapper;
using Shipping.ShippingPickupPoint.Domain;
using Shipping.ShippingPickupPoint.Models;

namespace Shipping.ShippingPickupPoint
{
    public class MapperConfiguration : Profile, IAutoMapperProfile
    {
        public int Order
        {
            get { return 0; }
        }

        public MapperConfiguration()
        {
            CreateMap<ShippingPickupPoints, ShippingPickupPointModel>()
            .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            .ForMember(dest => dest.StoreName, mo => mo.Ignore())
            .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
            .ForMember(dest => dest.UserFields, mo => mo.Ignore());

            CreateMap<ShippingPickupPointModel, ShippingPickupPoints>();
        }
    }
}
