namespace NextDnsIpMonitor.Services;

internal interface IPublicIpService
{
    Task<string> GetPublicIpAddressAsync(CancellationToken cancellationToken);
}