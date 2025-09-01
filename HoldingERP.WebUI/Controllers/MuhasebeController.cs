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
                .Where(t => t.Durum == TalepDurumu.MuhasebeSürecinde) 
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
            if (anaTalep != null && anaTalep.Durum != TalepDurumu.MuhasebeSürecinde)
            {
                TempData["ErrorMessage"] = "Bu talep fatura giriş aşamasında değil.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(FaturaNo) || FaturaTarihi == default)
            {
                TempData["ErrorMessage"] = "Fatura Numarası ve Tarihi boş olamaz.";
                return RedirectToAction("FaturaGir", new { id = TalepId });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var onaylanmisTeklif = _teklifService.GetById(TeklifId); // Sadece Id ile bulmak yeterli

            if (onaylanmisTeklif == null)
            {
                TempData["ErrorMessage"] = "İşlem yapılacak onaylı teklif bulunamadı.";
                return RedirectToAction("Index");
            }

            // 1. Yeni Fatura Nesnesini Oluştur ve Veritabanına Ekle
            var fatura = new Fatura
            {
                FaturaNo = FaturaNo,
                FaturaTarihi = FaturaTarihi,
                TeklifId = onaylanmisTeklif.Id,
                KaydedenKullaniciId = currentUser.Id,
                KayitTarihi = DateTime.Now
            };
            _faturaService.Create(fatura);

            // 2. Ana Talebin Durumunu "MuhasebeMüdürüOnayiBekliyor" Olarak Güncelle
            // Stok işlemleri bu aşamada yapılmayacak.
            if (anaTalep != null)
            {
                anaTalep.Durum = TalepDurumu.MuhasebeMüdürüOnayiBekliyor;
                _talepService.Update(anaTalep);
            }

            // 3. Tüm Değişiklikleri (yeni fatura ve durum güncellemesi) Veritabanına Yaz
            _talepService.SaveChanges();

            TempData["SuccessMessage"] = "Fatura bilgileri başarıyla kaydedildi ve Muhasebe Müdürü onayına gönderildi.";
            return RedirectToAction("Index");
        }
    }
}