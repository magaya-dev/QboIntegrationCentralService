using QboIntegrationCS.Domain.CosmosEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Repositories.Bills
{
    public interface IQboBillsCosmoRepo
    {
        Task<QboBillEntity> AddNewBillListAsync(QboBillEntity entity);
        Task UpdateBillAsync(QboBillEntity entity);
        Task<QboBillEntity> GetBillById(string transactionId, string networkId);
        Task<IEnumerable<QboBillEntity>> GetBillsBy(string networkId);
        Task DeleteBillAsync(string paymentId, QboBillEntity entity);
    }
}
