using HoldingERP.Business.Abstract;
using HoldingERP.DataAccess.Abstract;
using HoldingERP.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldingERP.Business.Concrete
{
    public class StokManager : GenericManager<Stok>, IStokService
    {
        public StokManager(IRepository<Stok> repository) : base(repository)
        {
            
        }
        public void FaturaIleStokGirisiYap(Fatura fatura, IEnumerable<TeklifKalem> kalemler, int talepId, int yapanKullaniciId)
        {
            
        }
    }
    }
