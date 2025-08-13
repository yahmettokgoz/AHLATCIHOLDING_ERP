using HoldingERP.Business.Abstract;
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Onaycı")]
    public class OnayController : Controller
    {
        private readonly IGenericService<SatinAlmaTalebi> _talepService;
        private readonly IGenericService<Teklif> _teklifService;
        private readonly UserManager<Kullanici> _userManager;

        public OnayController(
            IGenericService<SatinAlmaTalebi> talepService,
            IGenericService<Teklif> teklifService,
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

            var sorgu = _talepService.GetAllIncluding(
                talep => talep.TalepEdenKullanici.Departman
            );

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (isAdmin)
            {
                sorgu = sorgu.Where(t => t.Durum == TalepDurumu.AmirOnayiBekliyor ||
                                         t.Durum == TalepDurumu.YoneticiOnayiBekliyor);
            }
            else
            {
                sorgu = sorgu.Where(t =>
                    (t.Durum == TalepDurumu.AmirOnayiBekliyor && astlarinIdleri.Contains(t.TalepEdenKullaniciId)) ||
                    (t.Durum == TalepDurumu.YoneticiOnayiBekliyor)
                );
            }

            var onayBekleyenTalepler = await sorgu.ToListAsync();
            return View(onayBekleyenTalepler);
        }

        public IActionResult Details(int id)
        {
            var talep = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici).ThenInclude(k => k.Departman)
                .Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun)
                .FirstOrDefault(t => t.Id == id);

            if (talep == null)
            {
                return NotFound();
            }

            return View(talep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int talepId)
        {
            var talep = _talepService.GetById(talepId);
            if (talep == null)
            {
                return NotFound();
            }

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
            if (talep == null)
            {
                return NotFound();
            }

            talep.Durum = TalepDurumu.SatınAlmada;
            _talepService.Update(talep);
            _talepService.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
