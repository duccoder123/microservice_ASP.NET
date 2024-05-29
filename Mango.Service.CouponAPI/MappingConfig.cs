using AutoMapper;
using Mango.Service.CouponAPI.Models;
using Mango.Service.CouponAPI.Models.Dto;

namespace Mango.Service.CouponAPI
{
    public class MappingConfig : Profile
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
