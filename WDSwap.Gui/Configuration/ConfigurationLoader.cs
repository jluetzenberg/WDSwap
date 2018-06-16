using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDSwap.lib.Logging;

namespace WDSwap.Gui.Configuration
{
	public static class ConfigurationLoader
	{
		private const string FilePathTemplate = @"{0}\wdswap\config.ini";
		private static string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		private static ILogger _Logger = new Logger();
		public static string ConfigFilePath
		{
			get { return string.Format(FilePathTemplate, MyDocuments); }
		}


		public static WDSwapConfiguration LoadConfiguration()
		{
			EnsureConfigFileExists();

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(ConfigFilePath);

			//return GetConfigFromIniData(data);
			try
			{
				return WDSwapConfiguration.FromIni(data);
			}
			catch (Exception ex)
			{
				_Logger.Error("An error occurred while trying to read the configuration file",
					"An error occurred while trying to read the configuration file. This usually means that the file itself was not formatted correctly. Please review the error and attempt to repair the file, or else re-load the defaults");
				_Logger.Error(ex.Message, ex.ToString());

				var alert = MessageBox.Show("There was an error reading the configuration file. Loading default configuration. If this continues, please try to repair the configuration file or else re-load the defaults.", "ERROR!", MessageBoxButtons.OK);

				return GetDefault();
			}
		}

		public static void RestoreDefault()
		{
			CreateDefaultConfig();
		}

		private static void EnsureConfigFileExists()
		{
			string configDirectory = Path.GetDirectoryName(ConfigFilePath);
			if (Directory.Exists(configDirectory) == false)
			{
				Directory.CreateDirectory(configDirectory);
			}

			if (File.Exists(ConfigFilePath) == false)
			{
				CreateDefaultConfig();
			}
		}

		private static void CreateDefaultConfig()
		{
			var defaultConfig = GetDefault();
			var defaultIni = defaultConfig.ToIni();
			var parser = new FileIniDataParser();
			parser.WriteFile(ConfigFilePath, defaultIni);
		}

		private static WDSwapConfiguration GetDefault()
		{
			var result = new WDSwapConfiguration();

			result.FocusedDesktopColors.Background = "#FAFAFA";
			result.FocusedDesktopColors.Foreground = "#000000";
			result.FocusedDesktopColors.Border = "#FAFAFA";

			result.ActiveDesktopColors.Background = "#000000";
			result.ActiveDesktopColors.Foreground = "#FAFAFA";
			result.ActiveDesktopColors.Border = "#FAFAFA";

			result.InactiveDesktopColors.Background = "#A3A3A3";
			result.InactiveDesktopColors.Foreground = "#EAEAEA";
			result.InactiveDesktopColors.Border = "#EAEAEA";

			return result;
		}
	}
}
