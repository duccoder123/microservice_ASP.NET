
namespace Mango.Service.ShoppingCartAPI.Models.DTOs
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CardHeaderId { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDtos? Product { get; set; }
        public int Count { get; set; }
    }
}
