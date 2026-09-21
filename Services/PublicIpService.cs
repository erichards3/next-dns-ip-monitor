using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextDnsIpMonitor.Configuration;

namespace NextDnsIpMonitor.Services;

internal sealed class PublicIpService(
    HttpClient httpClient,
    IOptions<ServiceConfig> configuration,
    ILogger<PublicIpService> logger) : IPublicIpService
{
    public async Task<string> GetPublicIpAddressAsync(
        CancellationToken cancellationToken)
    {
        var endpoint = configuration.Value.IpAddressEndpoint;

        if (string.IsNullOrWhiteSpace(endpoint))
            throw new InvalidOperationException(
                "PublicIpMonitor:IpAddressEndpoint is not configured.");

        logger.LogDebug("Requesting public IP address from {Endpoint}.", endpoint);

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        response.EnsureSuccessStatusCode();

        var ipAddress = (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();

        logger.LogDebug("Public IP address returned: {IpAddress}.", ipAddress);

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new InvalidOperationException(
                "The public IP address endpoint returned an empty response.");
        }

        return ipAddress;
    }
}