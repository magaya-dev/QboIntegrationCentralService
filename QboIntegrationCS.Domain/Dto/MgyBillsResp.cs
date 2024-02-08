using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.Dto
{

    public class MgyBillsStatus
    {

        public string Id { get; set; }
        public string DocNumber { get; set; }
        public DateTime? TxnDate { get; set; }
        public bool Duplicated { get; set; }
        public bool MissingSqlLite { get; set; }
        public bool MissingCustomField { get; set; }
        public bool CustomFieldDifferent { get; set; }
        public bool MissingMagaya { get; set; }

        public MgyBillsStatus()
        {
            Duplicated = false;
            MissingSqlLite = false;
            MissingCustomField = false;
            CustomFieldDifferent = false;
            MissingMagaya = false;
        }
    }
}
