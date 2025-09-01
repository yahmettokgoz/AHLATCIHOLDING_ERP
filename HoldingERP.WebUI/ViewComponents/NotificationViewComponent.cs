using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HoldingERP.WebUI.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ITalepService _talepService;
        private readonly UserManager<Kullanici> _userManager;

        public NotificationViewComponent(
            ITalepService talepService, 
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return View(0);
            }

            var currentUser = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            if (currentUser == null)
            {
                return View(0);
            }

            var sorgu = _talepService.GetAll();
            int onayBekleyenTalepSayisi = 0;

            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                // Admin tüm onay bekleyenleri sayar
                onayBekleyenTalepSayisi = sorgu.Count(t =>
                    t.Durum == TalepDurumu.AmirOnayiBekliyor ||
                    t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || // YENİ
                    t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || // YENİ
                    t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor); // YENİ
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Onaycı"))
            {
                // Onaycı kendi astlarınınkini ve diğer yönetici onaylarını sayar
                var astlarinIdleri = await _userManager.Users
                                                       .Where(u => u.AmirId == currentUser.Id)
                                                       .Select(u => u.Id)
                                                       .ToListAsync();

                onayBekleyenTalepSayisi = sorgu.Count(t =>
                    (t.Durum == TalepDurumu.AmirOnayiBekliyor && astlarinIdleri.Contains(t.TalepEdenKullaniciId)) ||
                    t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || // YENİ
                    t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || // YENİ
                    t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor // YENİ
                );
            }

            return View(onayBekleyenTalepSayisi);
        }
    }
}