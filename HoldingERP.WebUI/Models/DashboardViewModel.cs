using HoldingERP.Entities.Concrete;

namespace HoldingERP.WebUI.Models
{
    public class DashboardViewModel
    {
        
        public int ToplamKullaniciSayisi { get; set; }
        public int OnayBekleyenTalepSayisi { get; set; }
        public int KullanicininAktifTalepSayisi { get; set; }
        public int IslemBekleyenSatinAlmaTalepSayisi { get; set; }
        public int TeklifBekleyenTalepSayisi { get; set; }
        public int FaturaBekleyenTalepSayisi { get; set; }
        public int ToplamUrunCesidi { get; set; }
        public IEnumerable<SatinAlmaTalebi>? SonTalepler { get; set; }
    }
}
