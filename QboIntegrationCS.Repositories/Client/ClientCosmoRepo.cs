using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using QboIntegrationCs.CosmosC.DataStorageClient;
using QboIntegrationCS.Domain.CosmosEntities;
using QboIntegrationCS.Repositories.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Repositories.Client
{
    public class ClientCosmoRepo : IClientCosmoRepo
    {
        private readonly ICosmosDBStorageClient _cosmoStorageClient;
        private readonly Container _container;
        private readonly string DB_TRANSACTION_CONTAINER = "QboClient";

        public ClientCosmoRepo (ICosmosDBStorageClient cosmoStorageClient)
        {
            _cosmoStorageClient = cosmoStorageClient;
            _container = _cosmoStorageClient.GetCosmosContainer(DB_TRANSACTION_CONTAINER).Result; 
        }

        public async Task<QboClientEntity> AddNewClientAsync(QboClientEntity entity)
        {
            var result = await _cosmoStorageClient.AddItemAsync(_container, entity);
            if (result == null)
            {
                return default;
            }
            return result.Resource;
        }

        public async Task UpdateClienAsync(QboClientEntity entity)
        {
            await _cosmoStorageClient.UpsertItemAsync(_container, entity);
        }

        public async Task<QboClientEntity> GetClientById(string networkId)
        {
            try
            {
                return await _cosmoStorageClient.GetItemBy<QboClientEntity>(_container, b => b.NetworkId == networkId);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
