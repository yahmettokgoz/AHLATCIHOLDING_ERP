using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Entities
{
    public class LogKaydi
    {
        public LogKaydi()
        {
            Zaman = DateTime.Now;
        }

        public int Id { get; set; }
        public string Islem { get; set; }     
        public string Nesne { get; set; }    
        public int? NesneId { get; set; }     
        public DateTime Zaman { get; set; }
        public int? KullaniciId { get; set; }
        public virtual Kullanici Kullanici { get; set; }


    }
}
