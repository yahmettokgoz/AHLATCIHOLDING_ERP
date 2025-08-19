using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class Bildirim
    {
        public Bildirim()
        {
            Tarih = DateTime.Now;
            OkunduMu = false; 
        }

        public int Id { get; set; }
        public string Mesaj { get; set; }
        public bool OkunduMu { get; set; }
        public DateTime Tarih { get; set; }
        public int KullaniciId { get; set; } 
        public virtual Kullanici Kullanici { get; set; }


    }
}
