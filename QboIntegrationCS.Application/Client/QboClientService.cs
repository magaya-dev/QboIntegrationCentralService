using QboIntegrationCS.Domain.CosmosEntities;
using QboIntegrationCS.Repositories.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.Client
{
    public class QboClientService : IQboClientService
    {
        private readonly IClientCosmoRepo _repo;

        public QboClientService(IClientCosmoRepo repo) 
        {
            _repo = repo;
        }

        public async Task CreateQboClient(string networkId, DateTime dtEnt)
        {
            await _repo.AddNewClientAsync(new QboClientEntity { 
                NetworkId = networkId,
                DtEnd = dtEnt
            } );
        }

        public async Task<DateTime?> GetDateClientQbo(string networkId)
        {
            var client = await _repo.GetClientById(networkId);
            if (client != null) return client.DtEnd;
            return null;
        }

        public async  Task UpdateClientQbo(string networkId, DateTime dtEnt) 
        {
            var client = await _repo.GetClientById(networkId);
            client.DtEnd = dtEnt;
            await _repo.UpdateClienAsync(client);
        }

    }
}
