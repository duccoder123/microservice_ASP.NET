using AutoMapper;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.DTOs;

namespace Mango.Service.ProductAPI
{
    public class MappingConfig : Profile
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDtos, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
