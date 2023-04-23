using DingDingRobotService.Models;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DingDingRobotService.Core
{
    public class RobotHelper
    {
        public static async ValueTask Send(RobotSetting setting, ILogger<object> logger)
        {
            if (!File.Exists("IPConfig.txt"))
            {
                logger.LogError("缺少IPConfig.txt配置文件");
                return;
            }
            setting.IPAddrs = File.ReadAllLines("IPConfig.txt");
            setting.InitAddr();
            var pingResult = await ToPingStr(setting);

            if (!string.IsNullOrEmpty(pingResult))
            {
                logger.LogInformation(pingResult);
                SendDingDingMsg(setting, pingResult);

            }
        }

        private static string? SendDingDingMsg(RobotSetting setting, string content)
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

        public static async Task<string?> ToPingStr(RobotSetting setting)
        {
            int failedNum = 0;
            int warningNum = 0;
            ConcurrentBag<string> pingBag = new ConcurrentBag<string>();
            //ConcurrentBag<string> DNSBag = new ConcurrentBag<string>();
            await Parallel.ForEachAsync(setting.IpSettings,new ParallelOptions { MaxDegreeOfParallelism=100 }, async (item, cancellationToken) =>
            {

                switch (await PingIp(item.Url, setting))
                {
                    case var s when s.Success == 0:
                        Interlocked.Increment(ref failedNum);
                        pingBag.Add(s.ToString() + "\r\n");
                        break;
                    case var s when s.Max > setting.PingWarningTime:
                        Interlocked.Increment(ref warningNum);
                        pingBag.Add(s.ToString() + "\r\n");
                        break;
                }
                //if (setting.IsAnalyzeUrl)//是否解析URL
                //{
                //    var (result, resStr) = await item.DNSAnalyze();
                //    if (result)
                //    {
                //        DNSBag.Add(resStr!);
                //    }
                //}

            });


            string? pingResult = null;
            if (pingBag.Count > 0)
            {
                pingResult = $"ping {setting.PingTimes}次,响应超过{(double)setting.PingWarningTime / 1000}秒的有{warningNum}个,响应失败的有{failedNum}个\r\n{string.Join("", pingBag.AsEnumerable())}";
            }
            //if (DNSBag.Count > 0)
            //{
            //    pingResult += "\r\n"+string.Join("", DNSBag.AsEnumerable());
            //}
            return pingResult;

        }
        private static async Task<PingResult> PingIp(string host, RobotSetting setting)
        {
            using (Ping pingSender = new Ping())
            {
                PingResult result = new PingResult { IpAddr = host, PingTimes = setting.PingTimes };
                List<long> roundtripTimeList = new List<long>();
                for (int i = 0; i < setting.PingTimes; i++)
                {
                    //模拟ping
                    try
                    {
                        PingReply reply = await pingSender.SendPingAsync(host, setting.PingTimeout);
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
}
