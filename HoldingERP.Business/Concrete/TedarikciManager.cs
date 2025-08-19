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
    public class TedarikciManager: GenericManager<Tedarikci>, ITedarikciService
    {
        public TedarikciManager(IRepository<Tedarikci> repository) : base(repository) { }
    }
}
