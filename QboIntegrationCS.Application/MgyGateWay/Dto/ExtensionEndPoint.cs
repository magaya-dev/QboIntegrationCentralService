using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.MgyGateWay.Dto
{
    public class ExtensionEndPoint
    {
        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("connection")]
        public string Connection { get; set; }

        [JsonPropertyName("local")]
        public string Local { get; set; }

        [JsonPropertyName("plus")]
        public bool Plus { get; set; }

        [JsonPropertyName("extensions")]
        public bool Extensions { get; set; }
    }
}
