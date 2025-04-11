var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUser = builder.AddParameter("rabbitmq-user", true);
var rabbitMqPassword = builder.AddParameter("rabbitmq-password", true);

var rabbitmq = builder.AddRabbitMQ("regis-pay-messaging", rabbitMqUser, rabbitMqPassword, port: 5672)
    .WithManagementPlugin();

var cosmos = builder.AddConnectionString("cosmos-db");

var api = builder.AddProject<Projects.Regis_Pay_Api>("regis-pay-api")
    .WithReference(cosmos);
builder.AddProject<Projects.Regis_Pay_ChangeFeed>("regis-pay-changefeed")
    .WithReference(rabbitmq)
    .WithReference(cosmos);
builder.AddProject<Projects.Regis_Pay_EventConsumer>("regis-pay-eventconsumer")
    .WithReference(cosmos)
    .WithReference(rabbitmq);
builder.AddProject<Projects.Regis_Pay_Demo>("regis-pay-demo")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
