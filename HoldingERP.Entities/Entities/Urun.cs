using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Entities
{
    public class Urun
    {

        public Urun()
        {
            TalepUrunleri = new HashSet<SatinAlmaTalepUrunu>();
            TeklifKalemleri = new HashSet<TeklifKalem>();
            Stoklar = new HashSet<Stok>();
            StokHareketleri = new HashSet<StokHareketi>();
        }

        public int Id { get; set; }
        public string Ad { get; set; }
        public string Birim { get; set; } 
        public string Aciklama { get; set; }
        public virtual ICollection<SatinAlmaTalepUrunu> TalepUrunleri { get; set; }
        public virtual ICollection<TeklifKalem> TeklifKalemleri { get; set; }
        public virtual ICollection<Stok> Stoklar { get; set; }
        public virtual ICollection<StokHareketi> StokHareketleri { get; set; }
    }
}
