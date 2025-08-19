using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class KullaniciRol
    {
        public int KullaniciId { get; set; }
        public int RolId { get; set; }
        public virtual Kullanici Kullanici { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
