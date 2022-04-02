# DingDingRobot
钉钉机器人消息通知---Ping服务器IP地址网络检测

> 配置参数

##### **appsettings.json**---`DingDingRobotSetting`节点配置

- **Secret**：配置钉钉机器人，选择加签方式时的密钥【必须选择加签方式】
- **SendUrl**：钉钉机器人webhook地址
- **PingTimes**：每个IP会ping多少次，默认10次
- **PingWarningTime**：IP进行ping返回的时间大于该值，则钉钉机器人发送通知（失败也会发送），单位毫秒，默认：1000
- **PingTimeout**：单位毫秒,ping的超时时间，默认：5000
- **PollingTime**：服务轮询时间【则为每一批次ping检测完后的停留时间】，单位毫秒，默认：10000

##### IPConfig.txt---配置IP，注意每个IP需要换行

> 发布

发布独立式应用。 创建 Windows 64 位可执行文件
`dotnet publish -c Release -r win-x64 `

更多publish选项请参考：https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-publish

