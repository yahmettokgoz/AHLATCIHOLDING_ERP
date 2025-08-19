using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public enum IslemTuru
    {
        Giris,   
        Cikis,   
        Transfer, 
        Sayim   
    }

    public class StokHareketi
    {
        public StokHareketi()
        {
            Tarih = DateTime.Now;
        }

        public int Id { get; set; }
        public decimal Miktar { get; set; } 
        public IslemTuru IslemTuru { get; set; }
        public DateTime Tarih { get; set; }

        public int UrunId { get; set; }
        public int IslemiYapanKullaniciId { get; set; }
        public int? FaturaId { get; set; } 
        public int? SatinAlmaTalebiId { get; set; } 
        public virtual Urun Urun { get; set; }
        public virtual Kullanici IslemiYapanKullanici { get; set; }
        public virtual Fatura Fatura { get; set; }
        public virtual SatinAlmaTalebi SatinAlmaTalebi { get; set; }

    }
}
