using Mango.Service.ShoppingCartAPI.Models.DTOs;

namespace Mango.Service.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDtos>> GetProductsAsync();
    }
}
