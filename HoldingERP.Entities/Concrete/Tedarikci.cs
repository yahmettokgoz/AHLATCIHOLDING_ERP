using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Entities.Concrete
{
    public class Tedarikci
    {
        public Tedarikci()
        {
            Teklifler = new HashSet<Teklif>();
        }

        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty; 
        public string IletisimBilgisi { get; set; } = string.Empty; 
        public virtual ICollection<Teklif> Teklifler { get; set; }  

        }
    }

