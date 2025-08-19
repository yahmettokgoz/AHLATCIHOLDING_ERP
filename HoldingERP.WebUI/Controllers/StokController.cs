using HoldingERP.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HoldingERP.WebUI.Controllers
{
    [Authorize(Roles = "Admin, Depo")]
    public class StokController : Controller
    {
        private readonly IStokService _stokService;

        public StokController(IStokService stokService)
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