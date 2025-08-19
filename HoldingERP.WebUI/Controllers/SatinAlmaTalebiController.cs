using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;
using HoldingERP.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize]
    public class SatinAlmaTalebiController : Controller
    {
        private readonly ITalepService _talepService;
        private readonly IUrunService _urunService;
        private readonly UserManager<Kullanici> _userManager;

        public SatinAlmaTalebiController(ITalepService talepService, IUrunService urunService, UserManager<Kullanici> userManager)
        {
            _talepService = talepService;
            _urunService = urunService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var talepler = _talepService.Find(t => t.TalepEdenKullaniciId == int.Parse(userId)).ToList();
            return View(talepler);
        }

        [Authorize(Roles = "Admin, Talep Eden")]
        public IActionResult Create()
        {
            var model = new TalepCreateViewModel { Urunler = _urunService.GetAll().ToList() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Talep Eden")]
        public async Task<IActionResult> Create(TalepCreateViewModel model)
        {
            model.Urunler = _urunService.GetAll().ToList();
            if (ModelState.IsValid && model.TalepUrunleri != null && model.TalepUrunleri.Any(p => p.UrunId > 0))
            {
                
                var currentUserId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(currentUserId))
                {                   
                    return Challenge();
                }

                var talep = new SatinAlmaTalebi
                {
                    Aciklama = model.Aciklama,
                    TalepTarihi = DateTime.Now,
                    Durum = TalepDurumu.AmirOnayiBekliyor,
                    TalepEdenKullaniciId = int.Parse(currentUserId) 
                };


                talep.TalepUrunleri = new List<SatinAlmaTalepUrunu>();
                foreach (var urunModel in model.TalepUrunleri)
                {
                    if (urunModel.UrunId > 0 && urunModel.Miktar > 0)
                    {
                        talep.TalepUrunleri.Add(new SatinAlmaTalepUrunu
                        {
                            UrunId = urunModel.UrunId,
                            Miktar = urunModel.Miktar
                        });
                    }
                }
                if (!talep.TalepUrunleri.Any())
                {
                    ModelState.AddModelError("", "Lütfen en az bir ürün ve miktar belirtiniz.");
                    return View(model);
                }
                _talepService.Create(talep);
                _talepService.SaveChanges();
                TempData["SuccessMessage"] = "Talebiniz başarıyla oluşturuldu ve onaya gönderildi.";
                return RedirectToAction(nameof(Index));
            }
            if (model.TalepUrunleri == null || !model.TalepUrunleri.Any())
            {
                model.TalepUrunleri = new List<TalepUrunModel> { new TalepUrunModel() };
            }
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var talep = _talepService.GetAll().Include(t => t.TalepUrunleri).ThenInclude(tu => tu.Urun).FirstOrDefault(t => t.Id == id);
            if (talep == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (talep.TalepEdenKullaniciId != int.Parse(userId) && !User.IsInRole("Admin")) return Forbid();
            return View(talep);
        }


        public IActionResult Edit(int id)
        {
            var talep = _talepService.GetAll()
                .Include(t => t.TalepUrunleri)
                .FirstOrDefault(t => t.Id == id);

            if (talep == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (talep.TalepEdenKullaniciId != int.Parse(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (talep.Durum != TalepDurumu.AmirOnayiBekliyor)
            {
                TempData["ErrorMessage"] = "Bu talep onay sürecinde olduğu için düzenlenemez.";
                return RedirectToAction(nameof(Index));
            }

            var model = new TalepCreateViewModel
            {
                Aciklama = talep.Aciklama,
                Urunler = _urunService.GetAll().ToList(), 
                TalepUrunleri = talep.TalepUrunleri.Select(tu => new TalepUrunModel
                {
                    UrunId = tu.UrunId,
                    Miktar = (int)tu.Miktar
                }).ToList()
            };

            ViewData["TalepId"] = id;

            return View(model);
        }


       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TalepCreateViewModel model)
        {
            model.Urunler = _urunService.GetAll().ToList();

            var talepToUpdate = _talepService.GetAll()
                                             .Include(t => t.TalepUrunleri)
                                             .FirstOrDefault(t => t.Id == id);

            if (talepToUpdate == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (talepToUpdate.TalepEdenKullaniciId != int.Parse(userId)) return Forbid();
            if (talepToUpdate.Durum != TalepDurumu.AmirOnayiBekliyor)
            {
                TempData["ErrorMessage"] = "Bu talep onay sürecinde olduğu için düzenlenemez.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                talepToUpdate.Aciklama = model.Aciklama;
                talepToUpdate.TalepUrunleri.Clear(); 

                foreach (var urunModel in model.TalepUrunleri)
                {
                    if (urunModel.UrunId > 0 && urunModel.Miktar > 0)
                    {
                        talepToUpdate.TalepUrunleri.Add(new SatinAlmaTalepUrunu
                        {
                            UrunId = urunModel.UrunId,
                            Miktar = urunModel.Miktar
                        });
                    }
                }

                _talepService.SaveChanges(); 

                TempData["SuccessMessage"] = "Talep başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["TalepId"] = id;
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var talep = _talepService.GetById(id);
            if (talep == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (talep.TalepEdenKullaniciId != int.Parse(userId)) return Forbid();
            if (talep.Durum != TalepDurumu.AmirOnayiBekliyor)
            {
                TempData["ErrorMessage"] = "Bu talep onay sürecinde olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }
            return View(talep);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var talep = _talepService.GetById(id);
            if (talep == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (talep.TalepEdenKullaniciId != int.Parse(userId))
            {
                return Forbid();
            }
            if (talep.Durum != TalepDurumu.AmirOnayiBekliyor)
            {
                TempData["ErrorMessage"] = "Bu talep onay sürecinde olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _talepService.Delete(talep);
            _talepService.SaveChanges();

            TempData["SuccessMessage"] = "Talep başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}