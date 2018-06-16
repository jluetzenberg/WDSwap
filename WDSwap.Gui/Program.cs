using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using WDSwap.lib.Logging;

namespace WDSwap.Gui
{
	static class Program
	{
		private static ILogger _Logger = new Logger();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += UnhandledException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var form = new SwapDisplay();
			form.Initialize();

			Application.Run(form);
		}


		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;

			_Logger.Critical("An unhandled error occurred in the application", "");

			if (ex != null)
			{
				_Logger.Critical(ex.Message, ex.ToString());
			}
		}
	}
}
