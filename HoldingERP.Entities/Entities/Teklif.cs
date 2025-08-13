using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Entities
{
    public class Teklif
    {
        public enum TeklifDurumu
        {
            Beklemede,   
            Secildi,     
            Onaylandi,   
            Reddedildi   
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
