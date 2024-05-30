using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

        public IActionResult Logout()
        {
            return View();
        }
    }
}
