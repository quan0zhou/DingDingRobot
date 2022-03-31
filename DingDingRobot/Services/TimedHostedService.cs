using DingDingRobot.Controllers;
using DingDingRobot.Core;
using DingDingRobot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _logger.LogDebug("服务启动");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMilliseconds(robotSetting.PollingTime));

            return Task.CompletedTask;
        }
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private async void DoWork(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogDebug("开始工作……");
            stopwatch.Restart();
            await RobotHelper.Send(robotSetting, _logger);
            stopwatch.Stop();
            _logger.LogDebug($"------------------总耗时：{(double)stopwatch.ElapsedMilliseconds/1000}秒---------------");
           _timer?.Change(TimeSpan.FromMilliseconds(robotSetting.PollingTime), TimeSpan.Zero);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("服务停止");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
