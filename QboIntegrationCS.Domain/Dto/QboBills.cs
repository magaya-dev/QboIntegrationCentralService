using QboIntegrationCS.Domain.ModelsEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.Dto
{
    public class QboBills
    {
        public PageInfo PageInfo { get; set; }
        public IEnumerable<BillDataResp> Bills { get; set; }
    }

    public class BillDataResp
    {
        public string Id { get; set; }
        public string DocNumber { get; set; }
        public DateTime? TxnDate { get; set; }
    }
}
