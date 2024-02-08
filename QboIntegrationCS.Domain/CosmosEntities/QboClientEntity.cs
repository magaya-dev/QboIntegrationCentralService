using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.CosmosEntities
{
    public class QboClientEntity : IEntity
    {
        [JsonProperty("_etag")]
        public string ETag { get; set; }


        [JsonProperty("id")]
        public string NetworkId { get; set; }
        public string PartitionKey => NetworkId;

        public DateTime DtEnd { get; set; }
    }
}
