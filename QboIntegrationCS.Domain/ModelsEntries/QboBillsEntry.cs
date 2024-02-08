using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Domain.ModelsEntries
{
    public class QboBillsEntry
    {
        public PageInfo PageInfo { get; set; }
        public BillData[] Objects { get; set; }
    }

    public class BillData
    {
        public TermRef? SalesTermRef { get; set; }
        public DateTime? DueDate { get; set; }
        public string Balance { get; set; } // int
        public string HomeBalance { get; set; } // int
        public string Domain { get; set; }
        public bool Sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public MetaDataInfo? MetaData { get; set; }
        public string DocNumber { get; set; }
        public DateTime? TxnDate { get; set; }
        public RefData? CurrencyRef { get; set; }
        public string ExchangeRate { get; set; }
        public LinkedTxnInfo[] LinkedTxn { get; set; }
        public LineInfo[] Line { get; set; }
        public RefData? VendorRef { get; set; }
        public RefData? APAccountRef { get; set; }
        public float? TotalAmt { get; set; }
        public string PrivateNote { get; set; }
    }

    public class TermRef
    {
        public string Value { get; set; }
    }

    public class MetaDataInfo
    {
        public DateTime? CreateTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }

    public class RefData
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }

    public class LinkedTxnInfo
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class LineInfo
    {
        public string Id { get; set; }
        public string LineNum { get; set; } // int?
        public string Description { get; set; }
        public float? Amount { get; set; }
        public object[] LinkedTxn { get; set; }
        public string DetailType { get; set; }
        public ItemBasedExpenseLineDetailInfo ItemBasedExpenseLineDetail { get; set; }
        public AccountBasedExpenseLineDetailInfo? AccountBasedExpenseLineDetail { get; set; }
    }

    public class ItemBasedExpenseLineDetailInfo
    {
        public string BillableStatus { get; set; }
        public RefData? ItemRef { get; set; }
        public float? UnitPrice { get; set; }
        public float? Qty { get; set; }
        public TermRef? TaxCodeRef { get; set; }
    }

    public class AccountBasedExpenseLineDetailInfo
    {
        public RefData? AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public TermRef? TaxCodeRef { get; set; }
    }
}
