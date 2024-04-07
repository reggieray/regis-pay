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
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(s => s.GetTypes())
                                          .Where(p => typeof(IConsumer).IsAssignableFrom(p) && p.IsClass &&
                                            !p.IsAbstract && p.Namespace!.Contains("Regis.Pay.EventConsumer."))
                                          .Select(x => x.Assembly)
                                          .ToArray();

                    x.AddConsumers(assemblies);
                }
            });
        }

        public static void AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new CosmosConfigOptions();
            configuration.GetSection(CosmosConfigOptions.Position).Bind(options);

            services.AddSingleton(options);
            services.AddSingleton(x =>
            {
                var options = x.GetRequiredService<CosmosConfigOptions>();
                var cosmosClient = CreateCosmosClient(options);

                return cosmosClient;
            });

            services.AddSingleton(x =>
            {
                var cosmosClient = x.GetRequiredService<CosmosClient>();

                Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(options.DatabaseName).GetAwaiter().GetResult();
                database.CreateContainerIfNotExistsAsync(new ContainerProperties(options.ContainerName, "/stream/id")).GetAwaiter().GetResult();
                database.CreateContainerIfNotExistsAsync(new ContainerProperties(options.LeasesContainerName, "/id")).GetAwaiter().GetResult();


                var storedProcedureProperties = new StoredProcedureProperties
                {
                    Id = "spAppendToStream",
                    Body = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/EventStore/js/spAppendToStream.js")
                };

                var eventsContainer = database.GetContainer(options.ContainerName);

                try
                {
                    eventsContainer.Scripts.ReplaceStoredProcedureAsync(storedProcedureProperties).GetAwaiter().GetResult();
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // Stored procedure didn't exist yet.
                    eventsContainer.Scripts.CreateStoredProcedureAsync(storedProcedureProperties).GetAwaiter().GetResult();
                }

                return eventsContainer;
            });
        }

        private static CosmosClient CreateCosmosClient(CosmosConfigOptions options)
        {
            return new CosmosClient(options.Endpoint, options.AuthKeyOrResourceToken, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                HttpClientFactory = () =>
                {
                    /*                               *** WARNING ***
                        * This code is for demo purposes only. In production, you should use the default behavior,
                        * which relies on the operating system's certificate store to validate the certificates.
                    */
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    return new HttpClient(httpMessageHandler);
                },
                ConnectionMode = ConnectionMode.Direct
            });
        }
    }
}
