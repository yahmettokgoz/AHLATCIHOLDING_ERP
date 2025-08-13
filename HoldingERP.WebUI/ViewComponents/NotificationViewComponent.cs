using HoldingERP.Business.Abstract;
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoldingERP.WebUI.ViewComponents
{
    public class NotificationViewComponent:ViewComponent
    {
        private readonly IGenericService<SatinAlmaTalebi> _talepService;
        private readonly UserManager<Kullanici> _userManager;

        public NotificationViewComponent(
            IGenericService<SatinAlmaTalebi> talepService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            if (currentUser == null)
            {
                return View(0); 
            }

            var astlarinIdleri = await _userManager.Users
                                                   .Where(u => u.AmirId == currentUser.Id)
                                                   .Select(u => u.Id)
                                                   .ToListAsync();

            var onayBekleyenTalepSayisi = _talepService.Find(
                t => t.Durum == TalepDurumu.AmirOnayiBekliyor &&
                     astlarinIdleri.Contains(t.TalepEdenKullaniciId)
            ).Count();

            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                onayBekleyenTalepSayisi = _talepService.Find(t => t.Durum == TalepDurumu.AmirOnayiBekliyor).Count();
            }

            return View(onayBekleyenTalepSayisi);
        }
    }
}
