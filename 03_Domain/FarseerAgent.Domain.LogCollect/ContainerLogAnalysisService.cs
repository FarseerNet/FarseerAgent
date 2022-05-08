using System.Globalization;
using System.Text.RegularExpressions;
using FarseerAgent.Domain.LogCollect.Container;
using FarseerAgent.Domain.LogCollect.ContainerLog;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FarseerAgent.Domain.LogCollect;

public class ContainerLogAnalysisService : ISingletonDependency
{
    /// <summary>
    /// 将字符串日志，转换成对象
    /// </summary>
    public ContainerLogDO Analysis(ContainerDO container, string log)
    {
        // 解析日期时间
        var time = Regex.Match(log.Split(' ')[0].Replace("T", " "), "[\\d-:\\s]+").Value;
        DateTime.TryParseExact(time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var create);

        var logLevel = LogLevel.Information;
        var logLower = log.ToLower();

        if (logLower.Contains("error")) logLevel        = LogLevel.Error;
        else if (logLower.Contains("warning")) logLevel = LogLevel.Warning;
        else if (logLower.Contains("debug")) logLevel   = LogLevel.Debug;

        var logSpaceIndex = log.IndexOf(' ');

        return new ContainerLogDO
        {
            LogLevel       = logLevel,
            Content        = logSpaceIndex > 0 ? log.Substring(logSpaceIndex) : log,
            CreateAt       = create,
            AppName        = container.App.Name,
            ContainerId    = container.Id,
            ContainerEnv   = container.Env,
            ContainerImage = container.Image,
            ContainerIp    = container.Ip,
            ContainerName  = container.Name,
            NodeIp         = container.Node.Ip,
            NodeName       = container.Node.Name
        };
    }
}