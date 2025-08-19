using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;


namespace HoldingERP.Entities.Concrete
{
   
    public class Kullanici : IdentityUser<int>
    {

        public Kullanici()
        {
            Talepler = new HashSet<SatinAlmaTalebi>();
            Onaylar = new HashSet<Onay>();
            GirilenTeklifler = new HashSet<Teklif>();
            GirilenFaturalar = new HashSet<Fatura>();
            YaptigiStokHareketleri = new HashSet<StokHareketi>();
            Bildirimler = new HashSet<Bildirim>();
            LogKayitlari = new HashSet<LogKaydi>();
        }


        public string AdSoyad { get; set; } = string.Empty;
        public bool AktifMi { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        public int? AmirId { get; set; }
        public int DepartmanId { get; set; }

        public virtual Departman? Departman { get; set; }
        public virtual Kullanici? Amir { get; set; }

        public virtual ICollection<SatinAlmaTalebi> Talepler { get; set; }
        public virtual ICollection<Onay> Onaylar { get; set; }
        public virtual ICollection<Teklif> GirilenTeklifler { get; set; }
        public virtual ICollection<Fatura> GirilenFaturalar { get; set; }
        public virtual ICollection<StokHareketi> YaptigiStokHareketleri { get; set; }
        public virtual ICollection<Bildirim> Bildirimler { get; set; }
        public virtual ICollection<LogKaydi> LogKayitlari { get; set; }

      
    }
}