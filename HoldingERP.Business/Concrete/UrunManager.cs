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
    public class UrunManager : GenericManager<Urun>, IUrunService
    {
        public UrunManager(IRepository<Urun> repository) : base(repository) { }
    }
}
