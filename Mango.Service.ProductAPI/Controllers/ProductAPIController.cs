using AutoMapper;
using Mango.Service.ProductAPI.Data;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Mango.Service.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _context.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDtos>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id}")]
        public ResponseDto GetById(int id) {
            try
            {
                Product coupon = _context.Products.First(x => x.ProductId == id);
                _response.Result = _mapper.Map<ProductDtos>(coupon);
            }
            catch (Exception ex) {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto CreateProduct([FromBody] ProductDtos product)
        {
            try
            {
                Product obj = _mapper.Map<Product>(product);
                _context.Products.Add(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<ProductDtos>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
          
        public ResponseDto UpdateProduct([FromBody] ProductDtos product)
        {
            try
            {
                Product obj = _mapper.Map<Product>(product);
                _context.Products.Update(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<ProductDtos>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]

        public ResponseDto DeleteProduct(int id)
        {
            try
            {
                Product obj = _context.Products.FirstOrDefault(x => x.ProductId == id);
                _context.Products.Remove(obj);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }
    }
}
