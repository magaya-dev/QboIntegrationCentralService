using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QboIntegrationCS.Application.Bill;
using QboIntegrationCS.Application.Client;
using QboIntegrationCS.Domain.AppSetting;

namespace QboIntegrationCS.Triggers.Timer
{
    public class UpdatedQboIDsTimerTrigger
    {
        private readonly IBillService _billService;
        private readonly IQboClientService _clientService;
        private readonly IOptions<ApplicationSettings> _settings;

        public UpdatedQboIDsTimerTrigger(IBillService service,
            IOptions<ApplicationSettings> settings,
            IQboClientService clientService)
        {
            _billService = service;
            _clientService = clientService;
            _settings = settings;
        }

        [FunctionName("UpdatedQboIDs")]
        public async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            //at 8:30 PM every day (0 30 20 * * *)
            log.LogInformation($"C# Timer UpdatedQboIDsTimerTrigger executed at: {DateTime.Now}");
            try
            {
                var networkId = _settings.Value.NetworkId;
                var token = _settings.Value.Token;
                var limit = _settings.Value.Limit;
                var dtStart = _settings.Value.DateStart;

                log.LogInformation($"NetworkId: {networkId}");

                if (!string.IsNullOrEmpty(networkId))
                {
                    //Get last date
                    var clientDate = await _clientService.GetDateClientQbo(networkId);
                    if (clientDate != null) dtStart = clientDate.Value;
                    else await _clientService.CreateQboClient(networkId, dtStart);

                    log.LogInformation($"dtStart: {dtStart}");

                    // get bills and send them to Mgy est
                    var respDate = await _billService.SendBills(networkId, token, limit, dtStart, null, "bills");

                    // Update last date
                    if (respDate != null)
                    {
                        await _clientService.UpdateClientQbo(networkId, respDate.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Timer trigger error: {ex.Message}");
                throw;
            }
        }
    }
}
