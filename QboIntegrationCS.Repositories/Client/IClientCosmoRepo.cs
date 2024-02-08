using QboIntegrationCS.Domain.CosmosEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Repositories.Client
{
    public interface IClientCosmoRepo
    {
        Task<QboClientEntity> AddNewClientAsync(QboClientEntity entity);
        Task UpdateClienAsync(QboClientEntity entity);
        Task<QboClientEntity> GetClientById(string networkId);
    }
}
