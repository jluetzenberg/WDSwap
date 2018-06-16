using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WDSwap.lib.Logging
{
	public class Logger : ILogger
	{
		private string _Directory;

		private string _LastLoadedFileName;



		private XmlDocument _LogXml = new XmlDocument();
		private const string LogFileTemplate = @"{0}\logs_{1}.xml";
		private const string RootName = "Logs";
		private string CurrentDay
		{
			get { return DateTime.Now.ToString("yyyyMMdd"); }
		}
		private string CurrentDatetime
		{
			get { return DateTime.Now.ToString("yyyyMMddTHHmm"); }
		}



		public Logger()
		{
			string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			_Directory = Path.Combine(documents, "wdswap", "Logs");
		}

		public Logger(string logDirectory)
		{
			_Directory = logDirectory;
		}

		public void Log(LogTypeEnum logType, string message, string properties)
		{
			string logFile = GetLogFileName();
			EnsureFileExists(logFile);

			var el = (XmlElement)_LogXml.DocumentElement.AppendChild(_LogXml.CreateElement("log"));
			el.SetAttribute("datecreated", CurrentDatetime);
			el.SetAttribute("logType", logType.ToString());
			el.AppendChild(_LogXml.CreateElement("message")).InnerText = message;
			el.AppendChild(_LogXml.CreateElement("properties")).InnerText = properties;

			_LogXml.Save(logFile);
		}

		public void ChangeLoggingDirectory(string directory)
		{
			_Directory = directory;
		}

		private string GetLogFileName()
		{
			return string.Format(LogFileTemplate, _Directory, CurrentDay);
		}

		private void EnsureFileExists(string logFile)
		{
			if (string.IsNullOrEmpty(_LastLoadedFileName) == false && _LastLoadedFileName.Equals(logFile))
				return;

			_LastLoadedFileName = logFile;

			if (File.Exists(logFile))
			{
				_LogXml.Load(logFile);
				return;
			}

			if (Directory.Exists(_Directory) == false)
				Directory.CreateDirectory(_Directory);
			CreateBaseLogFile(logFile);
		}

		private void CreateBaseLogFile(string logFile)
		{

			var xmlRoot = _LogXml.CreateElement(RootName);
			xmlRoot.SetAttribute("datecreated", CurrentDatetime);
			_LogXml.AppendChild(xmlRoot);
			_LogXml.Save(logFile);
		}
	}
}
