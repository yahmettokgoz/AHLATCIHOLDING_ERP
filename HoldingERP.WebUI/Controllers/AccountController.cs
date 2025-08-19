using HoldingERP.Business.Abstract; 
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize] 
    public class AccountController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;
        private readonly IDepartmanService _departmanService;

        public AccountController(
            UserManager<Kullanici> userManager,
            SignInManager<Kullanici> signInManager,
            IDepartmanService departmanService) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _departmanService = departmanService;
        }

        [AllowAnonymous] 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}