
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using QboIntegrationCs.CosmosC.DataStorageClient;
using QboIntegrationCS.Repositories.Bills;
using QboIntegrationCS.Application.Bill;
using QboIntegrationCS.Domain.AppSetting;
using QboIntegrationCS.Application.MgyGateWay;
using QboIntegrationCS.Repositories.Client;
using QboIntegrationCS.Application.Client;
//using QboIntegrationCs.CosmosDb.CosmosStorageClient;

[assembly: FunctionsStartup(typeof(QboIntegrationCS.Startup))]
namespace QboIntegrationCS
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("MgyExt", (s, httpClient) =>
            {
                var conf = s.GetService<IConfiguration>();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("target", "QBO");
            });

            builder.Services.AddSingleton<ICosmosDBStorageClient, CosmosDBStorageClient>(prov =>
            {
                var conf = prov.GetService<IConfiguration>();
                var dbConnection = conf.GetValue<string>("CosmosDbConnection");
                var dbName = conf.GetValue<string>("CosmosDbName");
                return new CosmosDBStorageClient(dbConnection, dbName);
            });

            builder.Services.AddSingleton<IQboBillsCosmoRepo, QboBillsCosmoRepo>();
            builder.Services.AddSingleton<IClientCosmoRepo, ClientCosmoRepo>();
            builder.Services.AddSingleton<IMgyGateWay, MgyGateWay>();

            builder.Services.AddScoped<IBillService, BillService>();
            builder.Services.AddScoped<IMgyBillDispatcher, MgyBillDispatcher>();
            builder.Services.AddScoped<IQboClientService, QboClientService>();

            builder
             .Services
             .AddOptions<ApplicationSettings>()
             .Configure<IConfiguration>((settings, configuration) =>
             {
                 configuration.Bind(settings);
                 var config = configuration.GetSection("ApplicationSettings");
                 builder.Services.Configure<ApplicationSettings>(config);
             });
        }
    }
}
