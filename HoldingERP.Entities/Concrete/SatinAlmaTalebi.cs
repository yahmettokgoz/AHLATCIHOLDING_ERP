using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public enum TalepDurumu
    {
        Beklemede = 0,
        AmirOnayiBekliyor = 1,
        Reddedildi = 2,
        SatınAlmada = 3,
        TeklifBekleniyor = 4,
        GenelMudurOnayiBekliyor = 5,
        YonetimKuruluOnayiBekliyor = 6,
        MuhasebeSürecinde = 7,
        MuhasebeMüdürüOnayiBekliyor = 8,
        FaturaKesildi = 9,
        StoktaMevcut = 10,
        Tamamlandi = 11
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
