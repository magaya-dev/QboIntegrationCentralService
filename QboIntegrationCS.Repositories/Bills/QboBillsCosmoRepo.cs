using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using QboIntegrationCs.CosmosC.DataStorageClient;
using QboIntegrationCS.Domain.CosmosEntities;

namespace QboIntegrationCS.Repositories.Bills
{
    public class QboBillsCosmoRepo: IQboBillsCosmoRepo
    {
        private readonly ICosmosDBStorageClient _cosmoStorageClient;
        private readonly Container _container;
        private readonly string DB_TRANSACTION_CONTAINER = "QboBills";
        private readonly ILogger<QboBillsCosmoRepo> _logger;

        public QboBillsCosmoRepo(ICosmosDBStorageClient cosmoStorageClient,
            ILogger<QboBillsCosmoRepo> logger)
        {
            _cosmoStorageClient = cosmoStorageClient;
            _logger = logger;
            _container = _cosmoStorageClient.GetCosmosContainer(DB_TRANSACTION_CONTAINER).Result;
        }

        public async Task<QboBillEntity> AddNewBillListAsync(QboBillEntity entity)
        {
            var result = await _cosmoStorageClient.AddItemAsync(_container, entity);
            if (result == null)
            {
                return default;
            }
            return result.Resource;

        }

        public async Task UpdateBillAsync(QboBillEntity entity)
        {
            await _cosmoStorageClient.UpsertItemAsync(_container, entity);
        }

        public async Task<QboBillEntity> GetBillById(string transactionId, string networkId)
        {
            try
            {
                return await _cosmoStorageClient.GetItemBy<QboBillEntity>(_container, b => b.NetworkId == networkId && b.TransactionId == transactionId);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<IEnumerable<QboBillEntity>> GetBillsBy(string networkId)
        {
            return await _cosmoStorageClient.GetItemsBy<QboBillEntity>(_container, b => b.NetworkId == networkId);
        }

        public async Task DeleteBillAsync(string paymentId, QboBillEntity entity)
        {
            await _cosmoStorageClient.DeleteItemAsync(_container, paymentId, entity);
        }

    }
}