using HoldingERP.Entities.Concrete;

namespace HoldingERP.WebUI.Models
{
    public class TeklifKarsilastirViewModel
    {
        public SatinAlmaTalebi Talep { get; set; }
        public List<Teklif> Teklifler { get; set; }
    }
}
