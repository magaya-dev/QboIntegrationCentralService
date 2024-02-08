using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.Dto
{
    public class MgyBills
    {
        public IEnumerable<BillDataResp> Entities { get; set; }
    }
}
