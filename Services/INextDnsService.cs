namespace NextDnsIpMonitor.Services;

internal interface INextDnsService
{
    Task NotifyAsync(string ipAddress, CancellationToken cancellationToken);
}