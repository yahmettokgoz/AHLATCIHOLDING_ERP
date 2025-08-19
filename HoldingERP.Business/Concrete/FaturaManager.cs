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
    public class FaturaManager: GenericManager<Fatura>, IFaturaService
    {
        public FaturaManager(IRepository<Fatura> repository) : base(repository) { }
    }
}
