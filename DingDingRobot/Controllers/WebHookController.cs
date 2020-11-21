using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DingDingRobot.Core;
using DingDingRobot.Models;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DingDingRobot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly RobotSetting robotSetting;
        private readonly ILogger<WebHookController> _logger;
        public WebHookController(IOptions<RobotSetting> option, ILogger<WebHookController> logger)
        {
            robotSetting = option.Value;
            _logger = logger;
        }

        [HttpGet("Send")]
        public async Task<string> Send()
        {

            return await RobotHelper.Send(robotSetting, _logger);
        }

        [HttpGet("Ping")]
        public async Task<string>  GetPingStr()
        {

            return await RobotHelper.ToPingStr(robotSetting.IPAddrs, robotSetting.PingTimes, robotSetting.PingWarningTime);

        }
    }
}
