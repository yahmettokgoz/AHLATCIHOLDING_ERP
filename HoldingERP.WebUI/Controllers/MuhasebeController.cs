using HoldingERP.Business.Abstract;
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static HoldingERP.Entities.Entities.Teklif;

namespace HoldingERP.WebUI.Controllers
{
        [Authorize(Roles = "Admin, Muhasebe")]
    public class MuhasebeController : Controller
    {

        private readonly IGenericService<SatinAlmaTalebi> _talepService;
        private readonly IGenericService<Teklif> _teklifService;
        private readonly IGenericService<Fatura> _faturaService;
        private readonly IGenericService<Stok> _stokService;
        private readonly IGenericService<StokHareketi> _stokHareketiService;
        private readonly UserManager<Kullanici> _userManager;

        public MuhasebeController(
            IGenericService<SatinAlmaTalebi> talepService,
            IGenericService<Teklif> teklifService,
            IGenericService<Fatura> faturaService,
            IGenericService<Stok> stokService,
            IGenericService<StokHareketi> stokHareketiService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _teklifService = teklifService;
            _faturaService = faturaService;
            _stokService = stokService;
            _stokHareketiService = stokHareketiService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var faturaBekleyenTalepler = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici)
                .ThenInclude(k => k.Departman)
                .Where(t => t.Durum == TalepDurumu.Onaylandi)
                .ToList();
            return View(faturaBekleyenTalepler);
        }

        public IActionResult FaturaGir(int id)
        {
            var onaylanmisTeklif = _teklifService.GetAll()
                .Include(t => t.Tedarikci)
                .Include(t => t.TeklifKalemleri).ThenInclude(tk => tk.Urun)
                .FirstOrDefault(t => t.SatinAlmaTalebiId == id && t.Durum == TeklifDurumu.Onaylandi);

            if (onaylanmisTeklif == null)
            {
                TempData["ErrorMessage"] = "Bu talebe ait onaylanmış bir teklif bulunamadı.";
                return RedirectToAction("Index");
            }
            var model = new FaturaGirViewModel
            {
                TalepId = id,
                OnaylanmisTeklif = onaylanmisTeklif
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FaturaGir(int TalepId, int TeklifId, string FaturaNo, DateTime FaturaTarihi)
        {
            var anaTalep = _talepService.GetById(TalepId);
            if (anaTalep != null && anaTalep.Durum == TalepDurumu.Tamamlandi)
            {
                TempData["ErrorMessage"] = "Bu talep zaten faturalandırılmış ve süreç tamamlanmıştır.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(FaturaNo) || FaturaTarihi == default)
            {
                TempData["ErrorMessage"] = "Fatura Numarası ve Tarihi boş olamaz.";
                return RedirectToAction("FaturaGir", new { id = TalepId });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var onaylanmisTeklif = _teklifService.GetAll()
                                                .Include(t => t.TeklifKalemleri)
                                                .FirstOrDefault(t => t.Id == TeklifId);

            if (onaylanmisTeklif == null)
            {
                TempData["ErrorMessage"] = "İşlem yapılacak onaylı teklif bulunamadı.";
                return RedirectToAction("Index");
            }

            var fatura = new Fatura
            {
                FaturaNo = FaturaNo,
                FaturaTarihi = FaturaTarihi,
                TeklifId = onaylanmisTeklif.Id,
                KaydedenKullaniciId = currentUser.Id,
                KayitTarihi = DateTime.Now
            };
            _faturaService.Create(fatura);
            _faturaService.SaveChanges();

            foreach (var kalem in onaylanmisTeklif.TeklifKalemleri)
            {
                var stok = _stokService.Get(s => s.UrunId == kalem.UrunId);
                if (stok == null)
                {
                    stok = new Stok { UrunId = kalem.UrunId, Miktar = kalem.Miktar, Lokasyon = "Merkez Depo", GuncellemeTarihi = DateTime.Now };
                    _stokService.Create(stok);
                }
                else
                {
                    stok.Miktar += kalem.Miktar;
                    stok.GuncellemeTarihi = DateTime.Now;
                    _stokService.Update(stok);
                }

                var hareket = new StokHareketi
                {
                    UrunId = kalem.UrunId,
                    Miktar = kalem.Miktar,
                    IslemTuru = IslemTuru.Giris,
                    FaturaId = fatura.Id,
                    Tarih = DateTime.Now,
                    IslemiYapanKullaniciId = currentUser.Id,
                    SatinAlmaTalebiId = TalepId
                };
                _stokHareketiService.Create(hareket);
            }

            if (anaTalep != null)
            {
                anaTalep.Durum = TalepDurumu.Tamamlandi;
                _talepService.Update(anaTalep);
            }

            _talepService.SaveChanges();

            TempData["SuccessMessage"] = "Fatura başarıyla kaydedildi ve ürünler stoğa eklendi.";
            return RedirectToAction("Index");
        }
    }
}
