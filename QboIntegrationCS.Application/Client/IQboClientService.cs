using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.Client
{
    public interface IQboClientService
    {
        Task CreateQboClient(string networkId, DateTime dtEnt);
        Task<DateTime?> GetDateClientQbo(string networkId);
        Task UpdateClientQbo(string networkId, DateTime dtEnt);
    }
}
