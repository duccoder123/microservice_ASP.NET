using AutoMapper;
using Mango.Service.ShoppingCartAPI.Data;
using Mango.Service.ShoppingCartAPI.Models;
using Mango.Service.ShoppingCartAPI.Models.DTOs;
using Mango.Service.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Service.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        private IProductService _productService;
        private ICouponService _couponService;
        public CartAPIController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;   
            _couponService = couponService; 
        }
        #region GetCart/userId
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(u => u.UserId == userId)),
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));
                IEnumerable<ProductDtos> product =await _productService.GetProductsAsync();
                foreach (var item in cart.CartDetails)
                {
                    item.Product = product.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                // apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCouponAsync(cart.CartHeader.CouponCode);
                    if(coupon is not null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                _response.Result= cart; 
            }
            catch (Exception ex)
            {
                _response.StatusMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;   
        }
        #endregion
        #region ApplyCoupon
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb =await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch(Exception ex)
            {
                _response.StatusMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
        #endregion
        #region RemoveCoupon
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.StatusMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
        #endregion
        #region CartUpsert
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto>? CartUpsert(CartDto cartDto)
        {

            try
            {
                var cartHeaderFrDb = await _context.CartHeaders.AsNoTracking().
                    FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFrDb is null) 
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _context.CartHeaders.Add(cartHeader);
                    await _context.SaveChangesAsync();  
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();  
                }
                else
                {
                    // check if details has same product
                    var cartDetailsFrDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => 
                    u.ProductId == cartDto.CartDetails.First().ProductId
                    && u.CartDetailsId == cartHeaderFrDb.CartHeaderId);
                    if(cartDetailsFrDb is null)
                    {
                        // create cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFrDb.CartHeaderId;
                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFrDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFrDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId   = cartDetailsFrDb.CartDetailsId;
                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.StatusMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
        #endregion
        #region RemoveCart
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto>? RemoveCart([FromBody] int cartDetailsId)
        {

            try
            {
                CartDetails cartDetails = _context.CartDetails
                    .First(u=> u.CartDetailsId == cartDetailsId);
                int totalCountofCartItem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if(totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.StatusMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
        #endregion
    }
}
