using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static HoldingERP.Entities.Concrete.Teklif;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Onaycı")]
    public class OnayController : Controller
    {
        private readonly ITalepService _talepService;
        private readonly ITeklifService _teklifService;
        private readonly UserManager<Kullanici> _userManager;

        public OnayController(
            ITalepService talepService,
            ITeklifService teklifService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _teklifService = teklifService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var astlarinIdleri = await _userManager.Users
                                                   .Where(u => u.AmirId == currentUser.Id)
                                                   .Select(u => u.Id)
                                                   .ToListAsync();

            var sorgu = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici)
                .ThenInclude(k => k.Departman);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            IQueryable<SatinAlmaTalebi> filtrelenmisSorgu; // Tip ataması için yeni değişken

            if (isAdmin)
            {
                // Admin tüm onayları görür
                filtrelenmisSorgu = sorgu.Where(t => t.Durum == TalepDurumu.AmirOnayiBekliyor ||
                                                     t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || // YENİ DURUM
                                                     t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || // YENİ DURUM
                                                     t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor); // YENİ DURUM
            }
            else // Sadece Onaycı
            {
                // Onaycı, kendi astlarının amir onaylarını ve tüm üst düzey onayları görür.
                filtrelenmisSorgu = sorgu.Where(t =>
                    (t.Durum == TalepDurumu.AmirOnayiBekliyor && astlarinIdleri.Contains(t.TalepEdenKullaniciId)) ||
                    t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || // YENİ DURUM
                    t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || // YENİ DURUM
                    t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor // YENİ DURUM
                );
            }

            var onayBekleyenTalepler = await filtrelenmisSorgu.ToListAsync();
            return View(onayBekleyenTalepler);
        }

        public IActionResult Details(int id)
        {
            var talep = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici).ThenInclude(k => k.Departman)
                .Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun)
                .FirstOrDefault(t => t.Id == id);

            if (talep == null) return NotFound();

            return View(talep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int talepId)
        {
            var talep = _talepService.GetById(talepId);
            if (talep == null) return NotFound();

            talep.Durum = TalepDurumu.Reddedildi;
            _talepService.Update(talep);
            _talepService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int talepId)
        {
            var talep = _talepService.GetById(talepId);
            if (talep == null) return NotFound();

            switch (talep.Durum)
            {
                case TalepDurumu.AmirOnayiBekliyor:
                    talep.Durum = TalepDurumu.SatınAlmada;
                    break;
                case TalepDurumu.GenelMudurOnayiBekliyor:
                case TalepDurumu.YonetimKuruluOnayiBekliyor:
                    talep.Durum = TalepDurumu.MuhasebeSürecinde; // YENİ DURUM
                    break;
                case TalepDurumu.MuhasebeMüdürüOnayiBekliyor:
                    // Muhasebe onayı sonrası, durumu "Onaylandi" olan seçilmiş teklifi bul.
                    var secilenTeklif = _teklifService.Get(t => t.SatinAlmaTalebiId == talepId && t.Durum == TeklifDurumu.Onaylandi); // HATA BURADAYDI

                    if (secilenTeklif != null)
                    {
                        // Bulunan bu teklifin durumunu "FaturaKesildi" olarak güncelle.
                        secilenTeklif.Durum = TeklifDurumu.FaturaKesildi;
                        _teklifService.Update(secilenTeklif);
                    }

                    // Ana talebin durumunu da "FaturaKesildi" yap.
                    talep.Durum = TalepDurumu.FaturaKesildi;
                    break;
            }

            _talepService.Update(talep);
            _talepService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}