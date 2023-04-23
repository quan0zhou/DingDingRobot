using DingDingRobotService;
using DingDingRobotService.Models;
using Microsoft.Extensions.Configuration;
using NLog.Web;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.Configure<RobotSetting>(builder.Configuration.GetSection("DingDingRobotSetting"));
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .UseNLog()
    .Build();


var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();


try
{

    await host.RunAsync();
}
catch (Exception ex)
{
    //NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();

}


