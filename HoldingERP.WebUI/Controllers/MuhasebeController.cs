using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static HoldingERP.Entities.Concrete.Teklif;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Muhasebe")]
    public class MuhasebeController : Controller
    {
        private readonly ITalepService _talepService;
        private readonly ITeklifService _teklifService;
        private readonly IFaturaService _faturaService;
        private readonly IStokService _stokService;
        private readonly UserManager<Kullanici> _userManager;

        public MuhasebeController(
            ITalepService talepService,
            ITeklifService teklifService,
            IFaturaService faturaService,
            IStokService stokService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _teklifService = teklifService;
            _faturaService = faturaService;
            _stokService = stokService;
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
                _stokService.FaturaIleStokGirisiYap(fatura, onaylanmisTeklif.TeklifKalemleri, TalepId, currentUser.Id);
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