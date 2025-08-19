using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class Fatura
    {
        public Fatura()
        {
            StokHareketleri = new HashSet<StokHareketi>();
            KayitTarihi = DateTime.Now;
        }

        public int Id { get; set; }
        public string FaturaNo { get; set; }
        public DateTime FaturaTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int TeklifId { get; set; } 
        public int KaydedenKullaniciId { get; set; } 
        public virtual Teklif Teklif { get; set; }
        public virtual Kullanici KaydedenKullanici { get; set; }
        public virtual ICollection<StokHareketi> StokHareketleri { get; set; }

    }
}
