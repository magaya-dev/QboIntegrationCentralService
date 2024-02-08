﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QboIntegrationCS.Application.MgyGateWay;
using QboIntegrationCS.Domain.AppSetting;
using QboIntegrationCS.Domain.Dto;
using QboIntegrationCS.Domain.ModelsEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCS.Application.Bill
{
    public interface IMgyBillDispatcher
    {
        Task<QboBills> GetQboBills(string networkId, string token, int limit, DateTime dtStart, int daysEnd, string endpoint, int offset);
        Task<IEnumerable<MgyBillsStatus>> DispatchBills(MgyBills mgyBills, string networkId, string token, string endpoint);
    }


    public class MgyBillDispatcher: IMgyBillDispatcher
    {
        private readonly IHttpClientFactory _client;
        private readonly ILogger<MgyBillDispatcher> _logger;
        

        public MgyBillDispatcher(IHttpClientFactory client,
            ILogger<MgyBillDispatcher> logger)
        {
            _client = client; ;
            _logger = logger;
        }

        public async Task<QboBills> GetQboBills(string networkId,
            string token,
            int limit,
            DateTime dtStart,
            int daysEnd,
            string endpoint,
            int offset)
        {
            try
            {
                var condtion = daysEnd != 0 ?
                    $"TxnDate > '{dtStart.ToString("yyyy-MM-dd")}' AND TxnDate < '{dtStart.AddDays(daysEnd).ToString("yyyy-MM-dd")}' order by TxnDate"
                    : $"TxnDate > '{dtStart.ToString("yyyy-MM-dd")}' order by TxnDate";
                _logger.LogInformation($"uri condition: {condtion}");

                var httpClient =  _client.CreateClient("MgyExt");
                var response = await httpClient.GetAsync(new Uri($"{endpoint}/bills?limit={limit}&offset={offset}" +
                    $"&token={token}&condition={condtion}")).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Response Success qbo bills: {await response.Content.ReadAsStringAsync()}");
                    var resp = JsonConvert.DeserializeObject<QboBillsEntry>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    if (resp.Objects != null)
                    {
                        return MappingBillsData(resp);
                    }
                }
                else
                {
                    _logger.LogError($"BadRequest Magaya qbo/bills: {response.StatusCode} | {response.ReasonPhrase} | error: {await response.Content?.ReadAsStringAsync()}");
                }
                return default;
            }
            catch (System.Exception exc)
            {
                _logger.LogError("Can't get qbo bills", exc.Message);
                throw exc;
            }
        }

        public async Task<IEnumerable<MgyBillsStatus>> DispatchBills(MgyBills mgyBills, string networkId, string token, string endpoint)
        {
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var stringContent = new StringContent(JsonConvert.SerializeObject(mgyBills), Encoding.UTF8, "application/json");
                var sss = JsonConvert.SerializeObject(mgyBills);

                _logger.LogInformation($"Sent bills to magy ext qbo: {JsonConvert.SerializeObject(mgyBills)}"); 

                var httpClient = _client.CreateClient("MgyExt");
                var response = await httpClient.PostAsync(new Uri($"{endpoint}/process-missing-entities/{networkId}?token={token}&entity=bills"), stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Response Success / DispatchBills ext: {await response.Content.ReadAsStringAsync()}");
                    return JsonConvert.DeserializeObject<IEnumerable<MgyBillsStatus>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
                else
                {
                    _logger.LogError($"BadRequest upload transaction to Magaya: {response.StatusCode} | {response.ReasonPhrase} error: {await response.Content?.ReadAsStringAsync()}");
                    return default;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError("Can't DispatchBills to Magaya ext", exc.Message);
                throw;
            }
        }

        private QboBills MappingBillsData(QboBillsEntry qboBills)
        {
            var result = new List<BillDataResp>();
            if (qboBills.Objects.Any())
            {
                foreach (var item in qboBills.Objects)
                {
                    result.Add(new BillDataResp
                    {
                        Id = item.Id,
                        DocNumber = item.DocNumber,
                        TxnDate = item.TxnDate.Value,
                    });
                }
                return new QboBills {
                    PageInfo = qboBills.PageInfo,
                    Bills = result
                };
            }
            return default; 
        }
    }
}
