using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Regis.Pay.Common.EventStore;
using System.Net;
using Regis.Pay.Common.Configuration;
using MassTransit;
using System.Reflection;

namespace Regis.Pay.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CosmosConfigOptions>(configuration.GetSection(CosmosConfigOptions.Position));
            services.AddSingleton<IEventStore, CosmosEventStore>();
        }

        public static void AddMessagingBus(this IServiceCollection services, IConfiguration configuration, bool addConsumers = false)
        {
            var options = new MassTransitConfigOptions();
            configuration.GetSection(MassTransitConfigOptions.Position).Bind(options);

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host, "/", h => {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });

                if (addConsumers) 
                {
                    var entryAssembly = Assembly.GetEntryAssembly();
                    x.AddConsumers(entryAssembly!);
                }
            });
        }

        public static async Task InitializeCosmos(IServiceCollection services, IConfiguration configuration)
        {
            var options = new CosmosConfigOptions();
            configuration.GetSection(CosmosConfigOptions.Position).Bind(options);

            var client = BuildCosmosClient(options);

            Database database = await client.CreateDatabaseIfNotExistsAsync(options.DatabaseName);
            await database.CreateContainerIfNotExistsAsync(new ContainerProperties(options.ContainerName, "/stream/id"));
            await database.CreateContainerIfNotExistsAsync(new ContainerProperties(options.LeasesContainerName, "/id"));


            var storedProcedureProperties = new StoredProcedureProperties
            {
                Id = "spAppendToStream",
                Body = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/EventStore/js/spAppendToStream.js")
            };

            var eventsContainer = database.GetContainer(options.ContainerName);

            try
            {
                await eventsContainer.Scripts.ReplaceStoredProcedureAsync(storedProcedureProperties);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Stored procedure didn't exist yet.
                await eventsContainer.Scripts.CreateStoredProcedureAsync(storedProcedureProperties);
            }

            services.AddSingleton(client);
            services.AddSingleton(options);
        }

        private static CosmosClient BuildCosmosClient(CosmosConfigOptions options)
        {
            return new CosmosClientBuilder(options.Endpoint, options.AuthKeyOrResourceToken)
                .Build();
        }
    }
}
