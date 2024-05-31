using Mango.Service.ShoppingCartAPI.Data;
using Mango.Service.ShoppingCartAPI.Models.DTOs;
using Mango.Service.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Service.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _clientFactory;
        public ProductService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<IEnumerable<ProductDtos>> GetProductsAsync()
        {
            var client = _clientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDtos>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDtos>(); 
        }
    }
}
