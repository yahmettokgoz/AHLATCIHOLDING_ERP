using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Entities
{
    public enum OnayTipi
    {
        Talep,
        Teklif
    }
    public enum OnayDurumu
    {
        Onaylandi,
        Reddedildi
    }
    public class Onay
    {

        public Onay()
        {
            OnayTarihi = DateTime.Now;
        }

        public int Id { get; set; }
        public OnayTipi OnayTipi { get; set; }
        public OnayDurumu Durum { get; set; }
        public DateTime OnayTarihi { get; set; }
        public string Yorum { get; set; } 
        public int OnaylayanKullaniciId { get; set; }

        public int? SatinAlmaTalebiId { get; set; } 
        public int? TeklifId { get; set; } 
        public virtual Kullanici OnaylayanKullanici { get; set; }
        public virtual SatinAlmaTalebi SatinAlmaTalebi { get; set; }
        public virtual Teklif Teklif { get; set; }

    }
}
