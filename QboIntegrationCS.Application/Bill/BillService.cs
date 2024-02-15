using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QboIntegrationCS.Application.MgyGateWay;
using QboIntegrationCS.Domain.CosmosEntities;
using QboIntegrationCS.Domain.Dto;
using QboIntegrationCS.Repositories.Bills;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.Bill
{
    public class BillService : IBillService
    {
        private readonly IMgyBillDispatcher _billDispatcher;
        private readonly IQboBillsCosmoRepo _billsCosmoRepo;
        private readonly IMgyGateWay _mgyGateWay;
        private readonly ILogger<BillService> _logger;

        public BillService(IMgyBillDispatcher billDispatcher,
            IQboBillsCosmoRepo billsCosmoRepo,
            IMgyGateWay mgyGateWay,
            ILogger<BillService> logger) 
        {
            _billDispatcher= billDispatcher;
            _billsCosmoRepo= billsCosmoRepo;
            _mgyGateWay = mgyGateWay;
            _logger = logger;
        }

        public async Task<DateTime?> SendBills(string networkId, string token, int limit, DateTime dtStart, int dtEnd, string transactionType)
        {
            try
            {
                //get magaya gateway url
                _logger.LogInformation($"Init magaya endpoint {networkId}");
                var endpoint = await _mgyGateWay.GetMgyCompanyEndpoint(networkId);
                _logger.LogInformation($"End magaya endpoint {networkId}, uri: {endpoint}");
                if (endpoint == null)
                {
                    _logger.LogError("Can't connect Magaya Gateway", networkId);
                    throw new System.Exception("Can't connect Magaya Gateway");
                }

                var offset = 1;
                // get qbo list
                var qboBills = await _billDispatcher.GetQboBills(networkId, token, limit, dtStart, dtEnd, endpoint, offset, transactionType);
                
                if (qboBills != null)
                {
                    var enddate = await ProccessBillsToMgy(qboBills, networkId, token, endpoint, dtStart, transactionType);
                    if (enddate == null || qboBills.PageInfo.TotalCount < 100)
                    {
                        // proceso normal
                        return enddate;
                    }
                    var runProccess = true;    
                    while (runProccess)
                    {
                        offset = qboBills.PageInfo.TotalCount + offset;
                        qboBills = await _billDispatcher.GetQboBills(networkId, token, limit, dtStart, dtEnd, endpoint, offset, transactionType);
                        if (qboBills != null)
                        {
                            DateTime? endDateWhile = await ProccessBillsToMgy(qboBills, networkId, token, endpoint, dtStart, transactionType);
                            if (endDateWhile == null)
                            {
                                runProccess = false;
                            } else
                            {
                                enddate = endDateWhile.Value;
                                if (qboBills.PageInfo.TotalCount < 100) runProccess = false;
                            }
                        }
                        else
                            runProccess = false;
                    }

                    return enddate;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendBills: {ex.Message}");
                throw;
            }
        }

        private async Task<DateTime?> ProccessBillsToMgy(QboBills qboBills, string networkId, string token, string endpoint, DateTime dtStart, string transactionType)
        {
            // save in cosmoDb
            var billsSaved = await _billsCosmoRepo.AddNewBillListAsync(MappingQboBill(qboBills,
                networkId,
                dtStart));

            // send mgyBills to ext,
            var billsResp = await _billDispatcher.DispatchBills(new MgyBills
            {
                Entities = qboBills.Bills.Where(b => !string.IsNullOrEmpty(b.DocNumber))
            }, networkId, token, endpoint, transactionType);

            // update bills status in cosmoDb
            if (billsResp != null)
            {
                billsSaved.Bills = MappingBillDataRespToEntity(billsResp, billsSaved.Bills);
                await _billsCosmoRepo.UpdateBillAsync(billsSaved);
                return billsResp.OrderByDescending(b => b.TxnDate).Select(b => b.TxnDate).First().Value;
            }
            return null;
        }

        private QboBillEntity MappingQboBill(QboBills qboBills, string networkId, DateTime dateStart)
        {
            return new QboBillEntity
            {
                NetworkId = networkId,
                TransactionId = Guid.NewGuid().ToString(),
                PageInfo = qboBills.PageInfo,
                Bills = qboBills.Bills.Any() ? MappingBillDataToEntity(qboBills.Bills) : null,
                DtStart = dateStart,
                DtEnd = GetLastBillDate(qboBills.Bills)
            };
        }

        private IEnumerable<BillData> MappingBillDataToEntity(IEnumerable<BillDataResp> billsResp)
        {
            var billsEntity = new List<BillData>();
            foreach (var item in billsResp)
            {
                billsEntity.Add(new BillData {  
                   QboId = item.Id,
                   DocNumber= item.DocNumber,
                   TxnDate = item.TxnDate,
                   MagayaRefNull = item.DocNumber == null ? true : false
                });
            }
            return billsEntity;
        }

        private DateTime GetLastBillDate(IEnumerable<BillDataResp> billsResp)
        {
            return billsResp.OrderByDescending(b => b.TxnDate).Select(b => b.TxnDate).First().Value;
        }

        private IEnumerable<BillData> MappingBillDataRespToEntity(IEnumerable<MgyBillsStatus> billsResp, IEnumerable<BillData> billsSource)
        {
            var billsEntity = new List<BillData>();
            foreach (var bill in billsSource)
            {
                if (billsResp.Select(b => b.Id).Contains(bill.QboId))
                {
                    var item = billsResp.First(b => b.Id == bill.QboId);
                    billsEntity.Add(new BillData
                    {
                        QboId = item.Id,
                        DocNumber = item.DocNumber,
                        TxnDate = item.TxnDate,
                        Duplicated = item.Duplicated,
                        MissingMagaya = item.MissingMagaya,
                        CustomFieldDifferent = item.CustomFieldDifferent,
                        MissingCustomField = item.MissingCustomField,
                        MissingSqlLite = item.MissingSqlLite
                    });
                } else
                {
                    billsEntity.Add(bill);
                }
            }
            return billsEntity;
        }
    }
}
