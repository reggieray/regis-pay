var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUser = builder.AddParameter("rabbitmq-user", true);
var rabbitMqPassword = builder.AddParameter("rabbitmq-password", true);

var rabbitmq = builder.AddRabbitMQ("regis-pay-messaging", rabbitMqUser, rabbitMqPassword, port: 5672)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin(15672)
    .WithEnvironment("RABBITMQ_PLUGINS_DIR", "/opt/rabbitmq/plugins:/additional-plugins")
    .WithEnvironment("RABBITMQ_ENABLED_PLUGINS_FILE", "/additional-plugins/rabbitmq_enabled_plugins")
    .WithBindMount(".rabbitmq-plugins", "/additional-plugins");

var cosmos = builder.AddConnectionString("cosmos-db");

var mocks = builder.AddProject<Projects.Regis_Pay_Mocks>("regis-pay-mocks")
    .WithHttpCommand(
        path: "/toggle-errors",
        displayName: "Toggle errors",
        commandOptions: new HttpCommandOptions()
        {
            Description = """            
                Toggles mocks endpoint to return errors.           
                """
        });

var api = builder.AddProject<Projects.Regis_Pay_Api>("regis-pay-api")
    .WithExternalHttpEndpoints()
    .WithReference(cosmos);

builder.AddProject<Projects.Regis_Pay_ChangeFeed>("regis-pay-changefeed")
    .WithReference(rabbitmq)
    .WithReference(cosmos);

builder.AddProject<Projects.Regis_Pay_EventConsumer>("regis-pay-eventconsumer")
    .WithReference(cosmos)
    .WithReference(rabbitmq)
    .WithReference(mocks);

builder.AddProject<Projects.Regis_Pay_Demo>("regis-pay-demo")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
