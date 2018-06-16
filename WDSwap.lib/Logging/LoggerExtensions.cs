using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.lib.Logging
{
	public static class LoggerExtensions
	{
		public static void Debug(this ILogger logger, string message, string properties)
		{
			logger.Log(LogTypeEnum.Debug, message, properties);
		}
		public static void Info(this ILogger logger, string message, string properties)
		{
			logger.Log(LogTypeEnum.Info, message, properties);
		}
		public static void Warning(this ILogger logger, string message, string properties)
		{
			logger.Log(LogTypeEnum.Warning, message, properties);
		}
		public static void Error(this ILogger logger, string message, string properties)
		{
			logger.Log(LogTypeEnum.Error, message, properties);
		}
		public static void Critical(this ILogger logger, string message, string properties)
		{
			logger.Log(LogTypeEnum.Critical, message, properties);
		}
	}
}
