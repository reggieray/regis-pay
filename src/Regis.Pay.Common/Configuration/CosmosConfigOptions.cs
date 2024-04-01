namespace Regis.Pay.Common.Configuration
{
    public class CosmosConfigOptions
    {
        public const string Position = "Cosmos";

        public string Endpoint { get; set; } = default!;

        public string AuthKeyOrResourceToken { get; set; } = default!;

        public string DatabaseName { get; set; } = default!;

        public string ContainerName { get; set; } = default!;

        public string LeasesContainerName { get; set; } = default!;
    }
}