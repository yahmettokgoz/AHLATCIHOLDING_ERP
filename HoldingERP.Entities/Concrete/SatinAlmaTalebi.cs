using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public enum TalepDurumu
    {
        Beklemede,
        AmirOnayiBekliyor,
        SatınAlmada,
        TeklifBekleniyor,
        YoneticiOnayiBekliyor,
        Onaylandi,
        Reddedildi,
        Tamamlandi
    }

    public class SatinAlmaTalebi
    {
        public SatinAlmaTalebi()
        {
            TalepUrunleri = new HashSet<SatinAlmaTalepUrunu>();
            Teklifler = new HashSet<Teklif>();
            Onaylar = new HashSet<Onay>();
            TalepTarihi = DateTime.Now; 
            Durum = TalepDurumu.Beklemede; 
        }

        public int Id { get; set; }
        public DateTime TalepTarihi { get; set; }
        public TalepDurumu Durum { get; set; }
        public string? Aciklama { get; set; }
        public int TalepEdenKullaniciId { get; set; }
        public virtual Kullanici TalepEdenKullanici { get; set; }
        public virtual ICollection<SatinAlmaTalepUrunu> TalepUrunleri { get; set; }
        public virtual ICollection<Teklif> Teklifler { get; set; }
        public virtual ICollection<Onay> Onaylar { get; set; }

    }
}
