using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.Bill
{
    public interface IBillService
    {
        Task<DateTime?> SendBills(string networkId, string token, int limit, DateTime dtStart, DateTime? dtEnd, string transactionType);
    }
}
