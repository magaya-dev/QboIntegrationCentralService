using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QboIntegrationCS.Application.MgyGateWay.Dto;
using QboIntegrationCS.Domain.AppSetting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.MgyGateWay
{
    public class MgyGateWay : IMgyGateWay
    {
        private readonly IHttpClientFactory _client;
        private readonly IOptions<ApplicationSettings> _settings;
        private readonly ILogger<MgyGateWay> _logger;

        public MgyGateWay(IHttpClientFactory client,
            IOptions<ApplicationSettings> settings,
            ILogger<MgyGateWay> logger) 
        {
            _client= client;
            _settings= settings;
            _logger= logger;
        }

        private static ConcurrentDictionary<string, string> _gatewayPathByNetworkId = new ConcurrentDictionary<string, string>();

        public async Task<string> GetMgyCompanyEndpoint(string networkId)
        {
            string connection;
            if (_gatewayPathByNetworkId.TryGetValue(networkId, out connection))
            {
                return connection;
            }

            var mgyGatewayEndpoint = _settings.Value.MgyGatewayEndpoint;
            var targetUri = string.Format(mgyGatewayEndpoint, networkId);
            _logger.LogInformation($"Uri access mgy: {targetUri}");
            // TODO: CACHE this
            var httpClient = _client.CreateClient("MgyExt");
            var response = await httpClient.GetAsync(targetUri).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
                ExtensionEndPoint extensionEndPoint = JsonConvert.DeserializeObject<ExtensionEndPoint>(responseStream);
                _gatewayPathByNetworkId.TryAdd(networkId, extensionEndPoint?.Connection);
                return extensionEndPoint?.Connection;
            }

            return null;
        }
    }
}
