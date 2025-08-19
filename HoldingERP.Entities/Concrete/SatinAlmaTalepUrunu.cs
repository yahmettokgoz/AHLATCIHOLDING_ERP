using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class SatinAlmaTalepUrunu
    {

        public int Id { get; set; }
        public decimal Miktar { get; set; }
        public int SatinAlmaTalebiId { get; set; }
        public int UrunId { get; set; }
        public virtual SatinAlmaTalebi SatinAlmaTalebi { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
