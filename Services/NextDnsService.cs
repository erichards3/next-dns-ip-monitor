using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextDnsIpMonitor.Configuration;

namespace NextDnsIpMonitor.Services;

internal sealed class NextDnsService(
    HttpClient httpClient,
    IOptions<ServiceConfig> configuration,
    ILogger<NextDnsService> logger) : INextDnsService
{
    public async Task NotifyAsync(
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var endpoint = configuration.Value.NotificationEndpoint;

        if (string.IsNullOrWhiteSpace(endpoint))
            throw new InvalidOperationException(
                "PublicIpMonitor:NotificationEndpoint is not configured.");

        logger.LogDebug(
            "IP address changed to {IpAddress}. Calling notification endpoint {Endpoint}.",
            ipAddress, endpoint);

        var response = await httpClient.GetAsync(
            endpoint,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        logger.LogInformation(
            "Successfully notified endpoint after public IP changed to {IpAddress}.", ipAddress);
    }
}
