using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DingDingRobotService.Models
{
    public class RobotSetting
    {

        public string Secret { get; set; } = default!;
        public string SendUrl { get; set; } = default!;

        public string[] IPAddrs { get; set; } = default!;

        public int PingTimes { get; set; }

        public int PingWarningTime { get; set; }

        public int PollingTime { get; set; }

        public int PingTimeout { get; set; }

        public bool IsAnalyzeUrl { get; set; }

        public List<IpSetting> IpSettings { get; set; } = default!;

        public void InitAddr()
        {
            IpSettings = new List<IpSetting>();
            foreach (var item in IPAddrs)
            {
                var index = item.Trim().IndexOf("#");
                if (index == 0)
                {
                    continue;
                }
                else
                {
                    #region 解析地址对比
                    //if (index > 0)
                    //{
                    //    var ipArray = item.Substring(0, index).Trim().Split("|");
                    //    if (ipArray.Length >= 2)
                    //    {
                    //        IpSettings.Add(new IpSetting(ipArray[0].Trim(), ipArray[1].Split(' ').Where(r => !string.IsNullOrEmpty(r)).Select(r => r.Trim()).ToArray()));
                    //    }
                    //    IpSettings.Add(new IpSetting(ipArray[0].Trim(), null));
                    //}
                    //else
                    //{
                    //    var ipArray = item.Trim().Split("|");
                    //    if (ipArray.Length >= 2)
                    //    {
                    //        IpSettings.Add(new IpSetting(ipArray[0].Trim(), ipArray[1].Split(' ').Where(r => !string.IsNullOrEmpty(r)).Select(r => r.Trim()).ToArray()));
                    //    }
                    //    IpSettings.Add(new IpSetting(ipArray[0].Trim(), null));
                    //}
                    #endregion

                    #region 仅仅只解析域名或IP
                    if (index > 0)
                    {
                        IpSettings.Add(new IpSetting(item.Substring(0, index).Trim(), null));
                    }
                    else
                    {
                        IpSettings.Add(new IpSetting(item.Trim(), null));
                    }
                    #endregion
                }
            }

        }

    }

    public struct IpSetting
    {
        public IpSetting(string url, string[]? analyzeIPs)
        {
            Url = url;
            AnalyzeIPs = analyzeIPs;
        }
        public string Url { get; set; }


        public string[]? AnalyzeIPs { get; set; }


        public async Task<(bool, string?)> DNSAnalyze()
        {

            if (AnalyzeIPs == null || AnalyzeIPs.Length <= 0)
            {
                return (false, null);
            }
            try
            {
                var addressArray = await Dns.GetHostAddressesAsync(Url);
                var result = false;
                foreach (var item in AnalyzeIPs)
                {
                    if (!addressArray.Any(r => r.ToString() == item))
                    {
                        result = true;
                        break;
                    }
                }
                StringBuilder stringBuilder = new StringBuilder();
                if (result)
                {
                    stringBuilder.Append($"{Url}\r\n • 解析有误。\r\n • 现解析为:");
                    if (addressArray.Length > 1)
                    {
                        for (int i = 0; i < addressArray.Length; i++)
                        {
                            if (i == 0)
                            {
                                stringBuilder.Append("\r\n");
                            }
                            stringBuilder.Append($" {(i + 1)}、{addressArray[i].ToString()}\r\n");
                        }
                    }
                    else
                    {
                        stringBuilder.Append(addressArray[0].ToString() + "\r\n");
                    }
                }
                else
                {
                    return (false, null);
                }
                return (true, stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                return (true, $"{Url}\r\n • 解析失败。\r\n");
            }


        }
    }
}
