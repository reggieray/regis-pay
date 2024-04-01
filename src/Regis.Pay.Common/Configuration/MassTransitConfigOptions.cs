namespace Regis.Pay.Common.Configuration
{
    public class MassTransitConfigOptions
    {
        public const string Position = "MassTransit";

        public string Host { get; set; } = default!;

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
