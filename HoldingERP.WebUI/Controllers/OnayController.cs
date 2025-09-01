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

            IQueryable<SatinAlmaTalebi> filtrelenmisSorgu;

            if (isAdmin)
            {
                filtrelenmisSorgu = sorgu.Where(t => t.Durum == TalepDurumu.AmirOnayiBekliyor ||
                                                     t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || 
                                                     t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || 
                                                     t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor); 
            }
            else 
            {
                filtrelenmisSorgu = sorgu.Where(t =>
                    (t.Durum == TalepDurumu.AmirOnayiBekliyor && astlarinIdleri.Contains(t.TalepEdenKullaniciId)) ||
                    t.Durum == TalepDurumu.GenelMudurOnayiBekliyor || 
                    t.Durum == TalepDurumu.YonetimKuruluOnayiBekliyor || 
                    t.Durum == TalepDurumu.MuhasebeMüdürüOnayiBekliyor 
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
                    talep.Durum = TalepDurumu.MuhasebeSürecinde; 
                    break;
                case TalepDurumu.MuhasebeMüdürüOnayiBekliyor:
                    var secilenTeklif = _teklifService.Get(t => t.SatinAlmaTalebiId == talepId && t.Durum == TeklifDurumu.Onaylandi); 

                    if (secilenTeklif != null)
                    {
                        secilenTeklif.Durum = TeklifDurumu.FaturaKesildi;
                        _teklifService.Update(secilenTeklif);
                    }

                    talep.Durum = TalepDurumu.FaturaKesildi;
                    break;
            }

            _talepService.Update(talep);
            _talepService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}