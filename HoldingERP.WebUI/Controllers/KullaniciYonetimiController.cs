using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KullaniciYonetimiController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly IDepartmanService _departmanService;

        public KullaniciYonetimiController(
            UserManager<Kullanici> userManager,
            RoleManager<Rol> roleManager,
            IDepartmanService departmanService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _departmanService = departmanService;
        }

        public async Task<IActionResult> Index()
        {
            var kullanicilar = await _userManager.Users.ToListAsync();
            return View(kullanicilar);
        }

        public IActionResult Create()
        {
            var model = new RegisterViewModel
            {
                Departmanlar = _departmanService.GetAll().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Kullanici
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    AdSoyad = model.FullName,
                    AktifMi = true,
                    OlusturmaTarihi = System.DateTime.Now,
                    DepartmanId = model.DepartmanId
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Talep Eden");
                    TempData["SuccessMessage"] = "Yeni kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Departmanlar = _departmanService.GetAll().ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditRolesViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName
            };

            foreach (var role in _roleManager.Roles.ToList())
            {
                var roleSelection = new RoleSelection
                {
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    roleSelection.IsSelected = true;
                }
                model.Roles.Add(roleSelection);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AssignAmir(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var amirAdaylari = await _userManager.Users
                                                 .Where(u => u.Id.ToString() != id)
                                                 .ToListAsync();

            var model = new AssignAmirViewModel { /* ... */ };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAmir(AssignAmirViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                user.AmirId = model.SecilenAmirId;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AmirAdaylari = await _userManager.Users
                                                   .Where(u => u.Id.ToString() != model.UserId)
                                                   .ToListAsync();
            return View(model);
        }
    }
}