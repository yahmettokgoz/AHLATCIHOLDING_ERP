using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Depo")]
    public class StokController : Controller
    {
        private readonly IGenericService<Stok> _stokService;

        public StokController(IGenericService<Stok> stokService)
        {
            _stokService = stokService;
        }

        public IActionResult Index()
        {
            
            var stokListesi = _stokService.GetAll()
                                          .Include(s => s.Urun)
                                          .ToList();

            return View(stokListesi);
        }
    }
}
