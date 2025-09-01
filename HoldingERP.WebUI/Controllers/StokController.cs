using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static HoldingERP.Entities.Concrete.Teklif;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Depo")]
    public class StokController : Controller
    {
        private readonly IStokService _stokService;
        private readonly IDepartmanService _departmanService; 
        private readonly IStokHareketiService _stokHareketiService; 
        private readonly UserManager<Kullanici> _userManager;
        private readonly ITalepService _talepService;
        private readonly ITeklifService _teklifService;
        private readonly IFaturaService _faturaService;
        

        public StokController(IStokService stokService, IDepartmanService departmanService, IStokHareketiService stokHareketiService, UserManager<Kullanici> userManager, ITalepService talepService, ITeklifService teklifService, IFaturaService faturaService) 
        {
            _stokService = stokService;
            _departmanService = departmanService;
            _stokHareketiService = stokHareketiService;
            _userManager = userManager;
            _talepService = talepService;
            _teklifService = teklifService;
            _faturaService = faturaService;
        }

        public IActionResult Index()
        {
            var stokListesi = _stokService.GetAll()
                                          .Include(s => s.Urun)
                                          .ToList();
            return View(stokListesi);
        }

        public IActionResult StokCikis(int id)
        {
            var stok = _stokService.GetAll().Include(s => s.Urun).FirstOrDefault(s => s.Id == id);
            if (stok == null) return NotFound();

            var model = new StokCikisViewModel
            {
                StokId = stok.Id,
                UrunAdi = stok.Urun.Ad,
                MevcutMiktar = stok.Miktar,
                Departmanlar = _departmanService.GetAll().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StokCikis(StokCikisViewModel model)
        {
            if (ModelState.IsValid)
            {
                var stok = _stokService.GetById(model.StokId);
                if (stok == null) return NotFound();

                if (model.CikisMiktari > stok.Miktar)
                {
                    ModelState.AddModelError("CikisMiktari", "Çıkış miktarı mevcut stoktan fazla olamaz.");
                    model.Departmanlar = _departmanService.GetAll().ToList(); 
                    return View(model);
                }

                var currentUser = await _userManager.GetUserAsync(User);

                stok.Miktar -= model.CikisMiktari;
                stok.GuncellemeTarihi = DateTime.Now;
                _stokService.Update(stok);

                var hareket = new StokHareketi
                {
                    UrunId = stok.UrunId,
                    Miktar = -model.CikisMiktari, 
                    IslemTuru = IslemTuru.Cikis,
                    Tarih = DateTime.Now,
                    IslemiYapanKullaniciId = currentUser.Id,
                };
                _stokHareketiService.Create(hareket);
                _stokService.SaveChanges();

                TempData["SuccessMessage"] = $"{model.CikisMiktari} adet {model.UrunAdi} stoktan başarıyla düşüldü.";
                return RedirectToAction("Index");
            }
            model.Departmanlar = _departmanService.GetAll().ToList();
            return View(model);
        }

        public IActionResult StokGirisBekleyenler()
        {
            var girisBekleyenTalepler = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici.Departman)
                .Where(t => t.Durum == TalepDurumu.FaturaKesildi)
                .ToList();

            return View(girisBekleyenTalepler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StokGirisiYap(int talepId)
        {
            var anaTalep = _talepService.GetById(talepId);
            if (anaTalep == null || anaTalep.Durum != TalepDurumu.FaturaKesildi)
            {
                return RedirectToAction(nameof(StokGirisBekleyenler));
            }

            var onaylanmisTeklif = _teklifService.GetAll() 
                                            .Include(t => t.TeklifKalemleri)
                                            .FirstOrDefault(t => t.SatinAlmaTalebiId == talepId && t.Durum == TeklifDurumu.FaturaKesildi);

            if (onaylanmisTeklif == null) { }

            var currentUser = await _userManager.GetUserAsync(User);
            var fatura = _faturaService.Get(f => f.TeklifId == onaylanmisTeklif.Id); 

            _stokService.FaturaIleStokGirisiYap(fatura, onaylanmisTeklif.TeklifKalemleri, talepId, currentUser.Id);

            anaTalep.Durum = TalepDurumu.StoktaMevcut;
            _talepService.Update(anaTalep);

            _talepService.SaveChanges();

            TempData["SuccessMessage"] = "Ürünler stoğa başarıyla eklendi.";
            return RedirectToAction(nameof(StokGirisBekleyenler));
        }

    }
}