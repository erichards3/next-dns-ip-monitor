using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NextDnsIpMonitor.Configuration;
using NextDnsIpMonitor.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "NextDNS IP Address Monitor";
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.Configure<ServiceConfig>(builder.Configuration.GetSection("ServiceConfig"));

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IPublicIpService, PublicIpService>();
builder.Services.AddSingleton<INextDnsService, NextDnsService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();
