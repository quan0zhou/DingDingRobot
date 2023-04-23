using DingDingRobotService.Core;
using DingDingRobotService.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DingDingRobotService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private RobotSetting _robotSetting;
        public Worker(ILogger<Worker> logger, IOptionsMonitor<RobotSetting> options)
        {
            _logger = logger;
            _robotSetting = options.CurrentValue;
            options.OnChange((setting, value) => {
                _robotSetting = setting;
            });
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("��������");
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                await RobotHelper.Send(_robotSetting, _logger);
                stopwatch.Stop();
                _logger.LogDebug($"------------------�ܺ�ʱ��{(double)stopwatch.ElapsedMilliseconds / 1000}��---------------");
                await Task.Delay(_robotSetting.PollingTime, stoppingToken);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("����ֹͣ");
            return base.StopAsync(cancellationToken);
        }

    }
}