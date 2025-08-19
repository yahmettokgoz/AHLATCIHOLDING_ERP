using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HoldingERP.Entities.Concrete.Teklif;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Satın Almacı")]
    public class SatinAlmaController : Controller
    {
        private readonly ITalepService _talepService;
        private readonly ITedarikciService _tedarikciService;
        private readonly ITeklifService _teklifService;
        private readonly ITalepUrunService _talepUrunService;
        private readonly ISatinAlmaService _satinAlmaService;
        private readonly UserManager<Kullanici> _userManager;

        public SatinAlmaController(
            ITalepService talepService,
            ITedarikciService tedarikciService,
            ITeklifService teklifService,
            ITalepUrunService talepUrunService,
            ISatinAlmaService satinAlmaService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _tedarikciService = tedarikciService;
            _teklifService = teklifService;
            _talepUrunService = talepUrunService;
            _satinAlmaService = satinAlmaService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var satinAlmadakiTalepler = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici.Departman)
                .Where(t => t.Durum == TalepDurumu.SatınAlmada)
                .ToList();
            return View(satinAlmadakiTalepler);
        }

        public IActionResult TeklifGir(int id)
        {
            var talep = _talepService.GetAll()
                .Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun)
                .FirstOrDefault(t => t.Id == id);

            if (talep == null || talep.Durum != TalepDurumu.SatınAlmada || !talep.TalepUrunleri.Any())
            {
                TempData["ErrorMessage"] = "Teklif girilebilecek uygun bir talep bulunamadı.";
                return RedirectToAction("Index");
            }

            var model = new TeklifGirViewModel
            {
                TalepId = talep.Id,
                TalepAciklamasi = talep.Aciklama,
                TalepUrunleri = talep.TalepUrunleri,
                GirilenTeklifler = new List<TeklifModel>
                {
                    new TeklifModel
                    {
                        TeklifKalemleri = talep.TalepUrunleri.Select(tu => new TeklifKalemModel
                        {
                            TalepUrunId = tu.Id,
                            UrunAdi = tu.Urun?.Ad ?? "Ürün Bulunamadı",
                            Miktar = tu.Miktar
                        }).ToList()
                    }
                }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeklifGir(TeklifGirViewModel model)
        {         
            var orjinalTalep = _talepService.GetAll()
                                            .Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun)
                                            .FirstOrDefault(t => t.Id == model.TalepId);

            if (orjinalTalep == null) return NotFound();

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var gecerliTeklifler = model.GirilenTeklifler
                    .Where(tm => !string.IsNullOrWhiteSpace(tm.TedarikciAdi) && tm.TeklifKalemleri.Any(k => k.BirimFiyat > 0))
                    .ToList();

                if (!gecerliTeklifler.Any())
                {
                    ModelState.AddModelError("", "Lütfen en az bir tedarikçi için geçerli bir fiyat teklifi giriniz.");
                }
                else
                {
                    foreach (var teklifModel in gecerliTeklifler)
                    {
                        var tedarikci = _tedarikciService.Get(t => t.Ad.ToLower() == teklifModel.TedarikciAdi.ToLower());
                        if (tedarikci == null)
                        {
                            tedarikci = new Tedarikci { Ad = teklifModel.TedarikciAdi, IletisimBilgisi = "Yeni Kayıt" };
                            _tedarikciService.Create(tedarikci);
                            _tedarikciService.SaveChanges(); 
                        }

                        var yeniTeklif = new Teklif
                        {
                            SatinAlmaTalebiId = model.TalepId,
                            TedarikciId = tedarikci.Id,
                            TeklifYapanKullaniciId = currentUser.Id,
                            TeklifTarihi = DateTime.Now,
                            Durum = TeklifDurumu.Beklemede,
                            ParaBirimi = "TRY",
                            TeklifKalemleri = new List<TeklifKalem>()
                        };

                       
                        foreach (var kalemModel in teklifModel.TeklifKalemleri)
                        {
                            var orjinalTalepUrunu = orjinalTalep.TalepUrunleri.FirstOrDefault(u => u.Id == kalemModel.TalepUrunId);
                            if (orjinalTalepUrunu != null && kalemModel.BirimFiyat > 0)
                            {
                                yeniTeklif.TeklifKalemleri.Add(new TeklifKalem
                                {
                                    UrunId = orjinalTalepUrunu.UrunId,
                                    Miktar = kalemModel.Miktar,
                                    BirimFiyat = kalemModel.BirimFiyat
                                });
                            }
                        }

                        yeniTeklif.ToplamFiyat = yeniTeklif.TeklifKalemleri.Sum(k => k.Miktar * k.BirimFiyat);
                        _teklifService.Create(yeniTeklif);
                    }

                    
                    orjinalTalep.Durum = TalepDurumu.TeklifBekleniyor;
                    _talepService.Update(orjinalTalep);
                    _talepService.SaveChanges();

                    TempData["SuccessMessage"] = "Teklifler başarıyla kaydedildi.";
                    return RedirectToAction("Index");
                }
            }

            model.TalepUrunleri = orjinalTalep.TalepUrunleri;
            model.TalepAciklamasi = orjinalTalep.Aciklama;

            foreach (var teklifModel in model.GirilenTeklifler)
            {
                foreach (var kalemModel in teklifModel.TeklifKalemleri)
                {
                    if (string.IsNullOrEmpty(kalemModel.UrunAdi))
                    {
                        var orjinalUrun = orjinalTalep.TalepUrunleri.FirstOrDefault(u => u.Id == kalemModel.TalepUrunId);
                        kalemModel.UrunAdi = orjinalUrun?.Urun?.Ad ?? "Bilinmeyen Ürün";
                    }
                }
            }

            return View(model);
        }


        public IActionResult TeklifleriDegerlendirListesi()
        {
            var teklifBekleyenTalepler = _talepService.GetAll()
                .Include(t => t.TalepEdenKullanici.Departman)
                .Where(t => t.Durum == TalepDurumu.TeklifBekleniyor)
                .ToList();
            return View(teklifBekleyenTalepler);
        }

        public IActionResult TeklifleriKarsilastir(int id)
        {
            
            var talep = _talepService.GetAll()
                .Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun)
                .FirstOrDefault(t => t.Id == id);

            if (talep == null)
            {
                return NotFound();
            }
 
            var teklifler = _teklifService.GetAll()
                .Where(t => t.SatinAlmaTalebiId == id)
                .Include(t => t.Tedarikci)
                .Include(t => t.TeklifKalemleri)
                .ToList();

            var model = new TeklifKarsilastirViewModel
            {
                Talep = talep,
                Teklifler = teklifler
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TeklifiSec(int secilenTeklifId)
        {
            var resultMessage = _satinAlmaService.TeklifiSecVeSüreciIlerlet(secilenTeklifId);
            TempData["SuccessMessage"] = resultMessage;
            return RedirectToAction("TeklifleriDegerlendirListesi");
        }
    }
}