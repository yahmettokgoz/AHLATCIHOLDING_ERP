using HoldingERP.Business.Abstract;
using HoldingERP.Entities;
using HoldingERP.Entities.Entities;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HoldingERP.Entities.Entities.Teklif;


namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Satın Almacı")]
    public class SatinAlmaController : Controller
    {
        private readonly IGenericService<SatinAlmaTalebi> _talepService;
        private readonly IGenericService<Tedarikci> _tedarikciService;
        private readonly IGenericService<Teklif> _teklifService;
        private readonly IGenericService<SatinAlmaTalepUrunu> _talepUrunService;
        private readonly UserManager<Kullanici> _userManager;

        public SatinAlmaController(
            IGenericService<SatinAlmaTalebi> talepService,
            IGenericService<Tedarikci> tedarikciService,
            IGenericService<Teklif> teklifService,
            IGenericService<SatinAlmaTalepUrunu> talepUrunService,
            UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _tedarikciService = tedarikciService;
            _teklifService = teklifService;
            _talepUrunService = talepUrunService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var satinAlmadakiTalepler = _talepService.GetAllIncluding(
                t => t.TalepEdenKullanici.Departman
            ).Where(t => t.Durum == TalepDurumu.SatınAlmada);

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

            var teklifKalemleri = talep.TalepUrunleri.Select(tu => new TeklifKalemModel
            {
                TalepUrunId = tu.Id,
                UrunAdi = tu.Urun?.Ad ?? "Ürün Bulunamadı",
                Miktar = tu.Miktar
            }).ToList();

            var model = new TeklifGirViewModel
            {
                TalepId = talep.Id,
                TalepAciklamasi = talep.Aciklama,
                TalepUrunleri = talep.TalepUrunleri,
                GirilenTeklifler = new List<TeklifModel>
                {
                    new TeklifModel { TeklifKalemleri = teklifKalemleri }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeklifGir(TeklifGirViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var orjinalTalepUrunleri = _talepUrunService.Find(t => t.SatinAlmaTalebiId == model.TalepId).ToList();

                foreach (var teklifModel in model.GirilenTeklifler)
                {
                    if (string.IsNullOrWhiteSpace(teklifModel.TedarikciAdi) || !teklifModel.TeklifKalemleri.Any(k => k.BirimFiyat > 0))
                        continue;

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
                        var orjinalUrun = orjinalTalepUrunleri.FirstOrDefault(u => u.Id == kalemModel.TalepUrunId);
                        if (orjinalUrun != null && kalemModel.BirimFiyat > 0)
                        {
                            yeniTeklif.TeklifKalemleri.Add(new TeklifKalem
                            {
                                UrunId = orjinalUrun.UrunId,
                                Miktar = kalemModel.Miktar,
                                BirimFiyat = kalemModel.BirimFiyat
                            });
                        }
                    }

                    yeniTeklif.ToplamFiyat = yeniTeklif.TeklifKalemleri.Sum(k => k.Miktar * k.BirimFiyat);
                    _teklifService.Create(yeniTeklif);
                }

                var talep = _talepService.GetById(model.TalepId);
                if (talep != null)
                {
                    talep.Durum = TalepDurumu.TeklifBekleniyor;
                    _talepService.Update(talep);
                }

                _talepService.SaveChanges();

                TempData["SuccessMessage"] = "Teklifler başarıyla kaydedildi.";
                return RedirectToAction("Index");
            }

            var orjinalTalep = _talepService.GetAll().Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun).FirstOrDefault(t => t.Id == model.TalepId);
            if (orjinalTalep != null)
            {
                model.TalepUrunleri = orjinalTalep.TalepUrunleri;
            }
            return View(model);
        }

        public IActionResult TeklifleriDegerlendirListesi()
        {
            var teklifBekleyenTalepler = _talepService.GetAllIncluding(
                t => t.TalepEdenKullanici.Departman
            ).Where(t => t.Durum == TalepDurumu.TeklifBekleniyor);

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

            var teklifler = _teklifService.GetAllIncluding(
                    t => t.Tedarikci,
                    t => t.TeklifKalemleri)
                .Where(t => t.SatinAlmaTalebiId == id)
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
            var secilenTeklif = _teklifService.GetById(secilenTeklifId);
            if (secilenTeklif == null) return NotFound();

            var anaTalep = _talepService.GetById(secilenTeklif.SatinAlmaTalebiId);
            if (anaTalep == null) return NotFound();

            var digerTeklifler = _teklifService
                .Find(t => t.SatinAlmaTalebiId == anaTalep.Id && t.Id != secilenTeklifId)
                .ToList();

            foreach (var teklif in digerTeklifler)
            {
                teklif.Durum = TeklifDurumu.Reddedildi;
                _teklifService.Update(teklif);
            }


            decimal limit = 100000; 

            if (secilenTeklif.ToplamFiyat > limit)
            {
                secilenTeklif.Durum = TeklifDurumu.Beklemede; 
                anaTalep.Durum = TalepDurumu.YoneticiOnayiBekliyor;

                TempData["SuccessMessage"] = $"Teklif seçildi. Tutar ({secilenTeklif.ToplamFiyat:C}) limiti aştığı için yönetici onayına gönderildi.";
            }
            else
            {
                secilenTeklif.Durum = TeklifDurumu.Onaylandi; 
                anaTalep.Durum = TalepDurumu.Onaylandi;

                TempData["SuccessMessage"] = $"Teklif başarıyla seçildi ve talep onaylandı.";
            }

            _teklifService.Update(secilenTeklif);
            _talepService.Update(anaTalep);

            _teklifService.SaveChanges();

            return RedirectToAction("TeklifleriDegerlendirListesi");
        }
    }
}