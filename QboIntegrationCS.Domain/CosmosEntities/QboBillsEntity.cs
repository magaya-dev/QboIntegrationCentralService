using Newtonsoft.Json;
using QboIntegrationCS.Domain.ModelsEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.CosmosEntities
{
    public class QboBillEntity : IEntity
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty("id")]
        public string TransactionId { get; set; }

        public string PartitionKey => NetworkId;

        public string NetworkId { get; set; }

        public PageInfo PageInfo { get; set; }

        public IEnumerable<BillData> Bills { get; set; }

        public DateTime DtStart { get; set; }
        public DateTime DtEnd { get; set; }

        public QboBillEntity()
        {
            Bills = new BillData[] { };
        }
    }

    public class BillData
    {
        
        public string QboId { get; set; } // Id
        public string DocNumber { get; set; }
        public DateTime? TxnDate { get; set; }
        public bool Duplicated { get; set; }
        public bool MissingSqlLite { get; set; }
        public bool MissingCustomField { get; set; }
        public bool CustomFieldDifferent { get; set; }
        public bool MissingMagaya { get; set; }
        public bool MagayaRefNull { get; set; }

        public BillData()
        {
            Duplicated = false;
            MissingSqlLite = false;
            MissingCustomField= false;
            CustomFieldDifferent = false;
            MissingMagaya = false;
            MagayaRefNull = false;
        }
    }



}
