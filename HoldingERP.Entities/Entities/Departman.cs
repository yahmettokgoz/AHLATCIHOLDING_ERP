using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Entities
{
    public class Departman
    {
        public Departman()
        {
            Kullanicilar = new HashSet<Kullanici>();
        }

        public int Id { get; set; }
        public string Ad { get; set; }

        public virtual ICollection<Kullanici> Kullanicilar { get; set; }


    }
}
