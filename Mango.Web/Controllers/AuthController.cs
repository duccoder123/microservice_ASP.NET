using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider; 
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider; 
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto LoginRequestDto = new();
            return View(LoginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {

            ResponseDto response = await _authService.LoginAsync(model);
            if (response is not null && response.IsSuccess)
            {
                LoginResponseDto loginRequestDto = 
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                await SignInUser(loginRequestDto);
                _tokenProvider.SetToken(loginRequestDto.Token);
                TempData["success"] = "Login Successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.StatusMessage;
                ModelState.AddModelError("CustomError", response.StatusMessage);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value =SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value =SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;    

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            ResponseDto? result = await _authService.RegisterAsync(model);
            ResponseDto assignRole;
            if(result is not null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(model.Role))
                {
                    model.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(model);
                if (assignRole is not null && assignRole.IsSuccess) {
                    TempData["success"] = "Registration Successfully";
                    return RedirectToAction(nameof(Login));
                }
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value =SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value =SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }

        private async Task SignInUser (LoginResponseDto model)
        {
            var handle = new JwtSecurityTokenHandler();
            var jwt = handle.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                 jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);   
        }
    }
}
