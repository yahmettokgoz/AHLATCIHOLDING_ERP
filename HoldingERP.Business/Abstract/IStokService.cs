using HoldingERP.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Business.Abstract
{
    public interface IStokService: IGenericService<Stok>
    {
        void FaturaIleStokGirisiYap(Fatura fatura, IEnumerable<TeklifKalem> kalemler, int talepId, int yapanKullaniciId);
    }
}
