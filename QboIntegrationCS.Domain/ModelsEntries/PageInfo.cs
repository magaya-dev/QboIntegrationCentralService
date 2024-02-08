using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.ModelsEntries
{
    public class PageInfo
    {
        public int MaxResults { get; set; }
        public int StartPosition { get; set; }
        public int TotalCount { get; set; }
    }

}
