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

        public string IPAddrs { get; set; }

        public int PingTimes { get; set; }

        public int PingWarningTime { get; set; }

        public int PollingTime { get; set; }

    }
}
