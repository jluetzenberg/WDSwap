using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDSwap.lib.Logging
{
	public interface ILogger
	{
		void ChangeLoggingDirectory(string directory);
		void Log(LogTypeEnum logType, string message, string properties);
	}
}
