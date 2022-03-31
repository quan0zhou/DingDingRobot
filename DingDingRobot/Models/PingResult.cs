using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DingDingRobot.Models
{
    public struct PingResult
    {
        /// <summary>
        /// ping的次数
        /// </summary>
        public int PingTimes { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// 失败
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// 丢包率
        /// </summary>
        public double LossRate { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string  IpAddr { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public long Min { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public long Max { get; set; }

        /// <summary>
        /// 平均值
        /// </summary>
        public long Avg{ get; set; }

        public override string ToString() 
        {
            return $"  • {this.IpAddr} | 成功:{this.Success},失败:{this.Failed},丢包率:{this.LossRate.ToString("0.00")}%,最小值:{this.Min},最大值{this.Max},平均值:{this.Avg}";
           
        }

    }
}
