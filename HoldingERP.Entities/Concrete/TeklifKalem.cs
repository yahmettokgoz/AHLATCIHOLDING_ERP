using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class TeklifKalem
    {
        public int Id { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public int TeklifId { get; set; }
        public int UrunId { get; set; }
        public virtual Teklif Teklif { get; set; }
        public virtual Urun Urun { get; set; }

    }
}
