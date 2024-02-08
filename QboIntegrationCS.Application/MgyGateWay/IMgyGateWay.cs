using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.MgyGateWay
{
    public interface IMgyGateWay
    {
        Task<string> GetMgyCompanyEndpoint(string networkId);
    }
}
