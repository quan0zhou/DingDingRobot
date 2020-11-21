using DingDingRobot.Models;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DingDingRobot.Core
{
    public class RobotHelper
    {
        public static async Task<string> Send(RobotSetting setting,ILogger<object> logger)
        {
            if (!File.Exists("IPConfig.txt"))
            {
                logger.LogError("缺少IPConfig.txt配置文件");
                return string.Empty;
            }
            setting.IPAddrs = File.ReadAllLines("IPConfig.txt");
            string content = await ToPingStr(setting.IPAddrs, setting.PingTimes, setting.PingWarningTime);
            if (!string.IsNullOrEmpty(content))
            {
                long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                string strSign = timestamp + "\n" + setting.Secret;
                string sign = string.Empty;
                using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(setting.Secret)))
                {
                    byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(strSign));
                    sign = HttpUtility.UrlEncode(Convert.ToBase64String(hashmessage), Encoding.UTF8);
                }

                IDingTalkClient client = new DefaultDingTalkClient($"{setting.SendUrl}&timestamp={timestamp}&sign={sign}");
                OapiRobotSendRequest.TextDomain text = new OapiRobotSendRequest.TextDomain()
                {
                    Content = content
                };
                OapiRobotSendRequest request = new OapiRobotSendRequest()
                {
                    Msgtype = "text",
                    Text_ = text

                };
                OapiRobotSendResponse response = client.Execute(request);
                return response.Body;
            }
            return string.Empty;
          
        }

        public static async Task<string> ToPingStr(string[] IpList,int pingTimes,int pingWarningTime)
        {
            int failedNum = 0;
            int warningNum = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var item in IpList)
            {
                switch (await PingIp(item, pingTimes))
                {
                    case var s when s.Success == 0:
                        failedNum++;
                        sb.Append(s.ToString() + "\r\n");
                        break;
                    case var s when s.Max > pingWarningTime:
                        warningNum++;
                        sb.Append(s.ToString() + "\r\n");
                        break;
                }
            }
            if (sb.Length>0)
            {
                return string.Concat($"ping {pingTimes}次,响应超过{pingWarningTime/1000}秒的有{warningNum}个,响应失败的有{failedNum}个\r\n", sb.ToString());
            }

            return string.Empty;
        }
        private static async Task<PingResult> PingIp(string host, int pingTimes)
        {
            Ping pingSender = new Ping();
            PingResult result = new PingResult { IpAddr = host, PingTimes = pingTimes };
            List<long> roundtripTimeList = new List<long>();
            for (int i = 0; i < pingTimes; i++)
            {
                //模拟ping
                try
                {
                    PingReply reply = await pingSender.SendPingAsync(host);
                    if (reply.Status != IPStatus.Success)
                    {
                        result.Failed += 1;
                    }
                    else
                    {
                        roundtripTimeList.Add(reply.RoundtripTime);
                    }
                }
                catch
                {

                    result.Failed += 1;
                }


            }
            result.Success = result.PingTimes - result.Failed;
            result.LossRate = ((double)result.Failed / (double)result.PingTimes) * 100;
            if (roundtripTimeList.Count > 0)
            {
                result.Min = roundtripTimeList.Min();
                result.Max = roundtripTimeList.Max();
                result.Avg = (long)roundtripTimeList.Average();
            }
            return result;
        }


    }
}
