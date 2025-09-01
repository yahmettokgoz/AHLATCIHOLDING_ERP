using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class Teklif
    {
        public enum TeklifDurumu
        {
            Beklemede,   // Satın almacı tarafından yeni girildi veya yönetici onayını bekliyor
            Secildi,     // Bu artık kullanılmıyor, kaldırılabilir veya tutulabilir.
            Onaylandi,   // Satın Alma Müdürü'nün seçtiği ve limit altı olduğu için onaylanan
            Reddedildi,  // Reddedilen
            FaturaKesildi // YENİ EKLENDİ - Muhasebe tarafından işlemi biten
        }

        public Teklif()
        {
            TeklifKalemleri = new HashSet<TeklifKalem>();
            Onaylar = new HashSet<Onay>();
            TeklifTarihi = DateTime.Now;
            Durum = TeklifDurumu.Beklemede;
        }
        public int Id { get; set; }
        public DateTime TeklifTarihi { get; set; }
        public decimal ToplamFiyat { get; set; }
        public string ParaBirimi { get; set; } 
        public TeklifDurumu Durum { get; set; }
        public int SatinAlmaTalebiId { get; set; }
        public int TedarikciId { get; set; }
        public int TeklifYapanKullaniciId { get; set; } 
        public virtual SatinAlmaTalebi SatinAlmaTalebi { get; set; }
        public virtual Tedarikci Tedarikci { get; set; }
        public virtual Kullanici TeklifYapanKullanici { get; set; }
        public virtual ICollection<TeklifKalem> TeklifKalemleri { get; set; }
        public virtual ICollection<Onay> Onaylar { get; set; }
        public virtual Fatura Fatura { get; set; } 


    }
}
