using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QboIntegrationCS.Application.Bill;
using Microsoft.Extensions.Options;
using QboIntegrationCS.Domain.AppSetting;

namespace QboIntegrationCS
{
    public class Function1
    { 
        private readonly IBillService _service;
        private readonly IOptions<ApplicationSettings> _settings;

        public Function1(IBillService service, IOptions<ApplicationSettings> settings) 
        {
            _service = service;
            _settings = settings;
        }

        [FunctionName("ManualProccess")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                var token = _settings.Value.Token;
                var limit = _settings.Value.Limit;
                var dtStart = DateTime.Parse(req.Query["dtStart"]);
                var dtEnd = _settings.Value.DaysEnd;
                var networkId = req.Query["networkId"];
                var transactionType = req.Query["transactionType"].ToString();
                if (string.IsNullOrEmpty(transactionType) || string.IsNullOrEmpty(transactionType))
                {
                    return new BadRequestObjectResult("NetworkId and TransactionType cannot be null or empty");
                }
                if (transactionType == "bills" || transactionType == "invoices")
                {
                    var respdate = await _service.SendBills(networkId, token, limit, dtStart, dtEnd, transactionType);

                    return new OkObjectResult($"Last bill date: {respdate}");
                } else
                {
                    return new BadRequestObjectResult("TransactionType value must be 'bills' or 'invoices'");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex.Message}");
                return new BadRequestObjectResult(ex.Message );
            }
            
        }
    }
}
