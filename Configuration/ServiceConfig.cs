namespace NextDnsIpMonitor.Configuration;

internal class ServiceConfig
{
    public int IntervalMinutes { get; set; } = 5;

    public string IpAddressEndpoint { get; set; } = string.Empty;

    public string NotificationEndpoint { get; set; } = string.Empty;
}
