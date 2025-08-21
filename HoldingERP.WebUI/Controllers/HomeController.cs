using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Kullanici> _userManager;
        private readonly ITalepService _talepService;
        private readonly IStokService _stokService;

        public HomeController(ILogger<HomeController> logger, UserManager<Kullanici> userManager, ITalepService talepService, IStokService stokService)
        {
            _logger = logger;
            _userManager = userManager;
            _talepService = talepService;
            _stokService = stokService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = new DashboardViewModel();

            // Tüm roller için ortak veriler
            model.ToplamKullaniciSayisi = _userManager.Users.Count();
            model.ToplamUrunCesidi = _stokService.GetAll().Count(); // Veya UrunService'den

            // --- Role Göre Veri Doldurma ---

            if (User.IsInRole("Admin") || User.IsInRole("Onaycý"))
            {
                model.OnayBekleyenTalepSayisi = _talepService.Find(t =>
                    t.Durum == TalepDurumu.AmirOnayiBekliyor || t.Durum == TalepDurumu.YoneticiOnayiBekliyor).Count();
            }

            if (User.IsInRole("Satýn Almacý"))
            {
                model.IslemBekleyenSatinAlmaTalepSayisi = _talepService.Find(t => t.Durum == TalepDurumu.SatýnAlmada).Count();
                model.TeklifBekleyenTalepSayisi = _talepService.Find(t => t.Durum == TalepDurumu.TeklifBekleniyor).Count();
            }

            if (User.IsInRole("Muhasebe"))
            {
                model.FaturaBekleyenTalepSayisi = _talepService.Find(t => t.Durum == TalepDurumu.Onaylandi).Count();
            }

            if (User.IsInRole("Talep Eden"))
            {
                model.KullanicininAktifTalepSayisi = _talepService.Find(t => t.TalepEdenKullaniciId == currentUser.Id && t.Durum != TalepDurumu.Tamamlandi).Count();
            }

            // Son 5 talebi de ekleyelim
            model.SonTalepler = _talepService.GetAll().OrderByDescending(t => t.TalepTarihi).Take(5).ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
