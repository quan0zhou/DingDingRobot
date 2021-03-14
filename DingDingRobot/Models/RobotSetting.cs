using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DingDingRobot.Models
{
    public class RobotSetting
    {

        public string Secret { get; set; }
        public string SendUrl { get; set; }

        public string[] IPAddrs { get; set; }

        public int PingTimes { get; set; }

        public int PingWarningTime { get; set; }

        public int PollingTime { get; set; }

        public void InitAddr()
        {
            List<string> ipList = new List<string>();
            foreach (var item in IPAddrs)
            {
                var index = item.Trim().IndexOf("#");
                if (index == 0)
                {
                    continue;
                }
                else
                {
                    if (index > 0)
                    {
                        ipList.Add(item.Substring(0, index).Trim());
                    }
                    else
                    {
                        ipList.Add(item.Trim());
                    }
                }
            }
            this.IPAddrs = ipList.ToArray();

        }

    }
}
