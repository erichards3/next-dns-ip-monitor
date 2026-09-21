using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextDnsIpMonitor.Configuration;

namespace NextDnsIpMonitor.Services;

internal sealed class Worker(
    IPublicIpService publicIpService,
    INextDnsService endpointNotifier,
    IOptions<ServiceConfig> options,
    ILogger<Worker> logger) : BackgroundService
{
    private string? _lastKnownIp;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NextDNS IP monitor started. Checking every {IntervalMinutes} minute(s).",
            options.Value.IntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckIpAddressAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking the public IP address.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(options.Value.IntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("NextDNS IP monitor stopped.");
    }

    private async Task CheckIpAddressAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Checking current public IP address.");

        var currentIp = await publicIpService.GetPublicIpAddressAsync(cancellationToken);

        if (string.Equals(currentIp, _lastKnownIp, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogDebug("Public IP has not changed. Current IP: {IpAddress}.", currentIp);

            return;
        }

        logger.LogInformation("Public IP changed from {PreviousIp} to {CurrentIp}.",
            _lastKnownIp ?? "(none)", currentIp);

        await endpointNotifier.NotifyAsync(currentIp, cancellationToken);

        _lastKnownIp = currentIp;
    }
}