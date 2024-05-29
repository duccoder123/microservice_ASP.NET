using AutoMapper;
using Mango.Service.CouponAPI.Data;
using Mango.Service.CouponAPI.Models;
using Mango.Service.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Mango.Service.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext context, IMapper mapper)
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
                IEnumerable<Coupon> objList = _context.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
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
                Coupon coupon = _context.Coupons.SingleOrDefault(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex) {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _context.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public ResponseDto CreateCoupon([FromBody] CouponDto coupon)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(coupon);
                _context.Coupons.Add(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusMessage = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Update(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
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
        public ResponseDto DeleteCoupon(int id)
        {
            try
            {
                Coupon obj = _context.Coupons.FirstOrDefault(x => x.CouponId == id);
                _context.Coupons.Remove(obj);
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
