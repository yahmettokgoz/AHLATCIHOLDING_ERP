using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmanController : Controller
    {
        private readonly IGenericService<Departman> _departmanService;

        public DepartmanController(IGenericService<Departman> departmanService)
        {
            _departmanService = departmanService;
        }
        public IActionResult Index()
        {
            var departmanlar = _departmanService.GetAll();
            return View(departmanlar);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Create(Departman departman)
        {
            
            if (ModelState.IsValid)
            {
                _departmanService.Create(departman);      
                _departmanService.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            
            return View(departman);
        }

        public IActionResult Edit(int id)
        {
            var departman = _departmanService.GetById(id);
            if (departman == null)
            {
                return NotFound();
            }
            return View(departman);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Departman departman)
        {
            
            if (id != departman.Id)
            {
                return NotFound();
            }
          
            if (ModelState.IsValid)
            {
                try
                {
                    _departmanService.Update(departman);
                    _departmanService.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                { 
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
 
            return View(departman);
        }

        public IActionResult Delete(int id)
        {
            var departman = _departmanService.GetById(id);
            if (departman == null)
            {
                return NotFound();
            }
            return View(departman);
        }

        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var departman = _departmanService.GetById(id);

            if (departman != null)
            {
                _departmanService.Delete(departman);
                _departmanService.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
