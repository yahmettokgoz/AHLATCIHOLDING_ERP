using HoldingERP.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoldingERP.Business.Abstract;
using HoldingERP.Entities.Concrete;

namespace HoldingERP.Business.Concrete
{
    public class StokHareketiManager: GenericManager<StokHareketi>, IStokHareketiService
    {
        public StokHareketiManager(IRepository<StokHareketi> repository) : base(repository) { }
    }
}
