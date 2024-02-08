using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.AppSetting
{
    public class ApplicationSettings
    {
        public string CosmosDbConnection { get; set; }
        public string CosmosDbName { get; set; }
        public string MgyGatewayEndpoint { get; set; }
        public int Limit { get; set; }
        public string Token { get; set; }
        public DateTime DateStart { get; set; }
        public int DaysEnd { get; set; }
        public string NetworkId { get; set; }
    }
}
 