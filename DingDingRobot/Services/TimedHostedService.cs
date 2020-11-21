using DingDingRobot.Controllers;
using DingDingRobot.Core;
using DingDingRobot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DingDingRobot.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly RobotSetting robotSetting;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IOptions<RobotSetting> option)
        {
            _logger = logger;
            robotSetting = option.Value;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(robotSetting.PollingTime));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Timed Hosted Service is working");
            Task.Run(async () =>
             {
                 await RobotHelper.Send(robotSetting);
             }).Wait();
            _logger.LogInformation("Timed Hosted Service is worked");
            _timer?.Change(TimeSpan.FromSeconds(robotSetting.PollingTime), TimeSpan.Zero);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
