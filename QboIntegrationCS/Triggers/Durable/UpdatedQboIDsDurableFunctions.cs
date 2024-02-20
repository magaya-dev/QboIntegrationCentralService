using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QboIntegrationCS.Application.Bill;
using QboIntegrationCS.Domain.AppSetting;

namespace QboIntegrationCS.Triggers.Durable
{
    

    public class UpdatedQboIDsDurableFunctions
    {
        private readonly IBillService _service;
        private readonly IOptions<ApplicationSettings> _settings;

        public UpdatedQboIDsDurableFunctions(IBillService service, IOptions<ApplicationSettings> settings)
        {
            _service = service;
            _settings = settings;
        }

        [FunctionName("UpdatedQboIDsDurableFunctions")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [FunctionName("UpdatedQboIDsDurableFunctions_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req, HttpRequest req1,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var token = _settings.Value.Token;
            var limit = _settings.Value.Limit;
            var dtStart = DateTime.Parse(req1.Query["dtStart"]);
            var dtEnd = _settings.Value.DaysEnd;
            var networkId = req1.Query["networkId"];
            var transactionType = req1.Query["transactionType"].ToString();
            //if (string.IsNullOrEmpty(transactionType) || string.IsNullOrEmpty(transactionType))
            //{
            //    return new Http("NetworkId and TransactionType cannot be null or empty");
            //}

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("UpdatedQboIDsDurableFunctions",null, token);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

    }

    public class DataInput
    {
        public string NetworkId { get; set; }
        public string TransactionType { get; set; }
        public string Token { get; set; }
        public int Limit { get; set; }
        public DateTime DtStart { get; set; }
        public DateTime? dtEnd { get; set; }
    }
}