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
    public class TalepManager: GenericManager<SatinAlmaTalebi>, ITalepService
    {
        public TalepManager(IRepository<SatinAlmaTalebi> repository) : base(repository) { }
    }
}
