using System;
using System.IO;

namespace ApiUtilLib
{
	public class FileLogger : LoggerBase
	{
		readonly string _messageFormat = "{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1} : {2}";

		public FileLogger()
		{
			this.LogLevel = LogLevel.None;
		}

		public FileLogger(LogLevel logLevel)
		{
			this.LogLevel = logLevel;
		}

		~FileLogger()
		{
			WriteLog(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1}", DateTime.Now, new string('-', 128)));
		}

		public override void LogMessage(LogLevel messageLogLevel, string message)
		{
			//Console.WriteLine(_messageFormat, DateTime.Now, messageLogLevel.ToString(), message);
			WriteLog(string.Format(_messageFormat, DateTime.Now, messageLogLevel.ToString(), message));
		}

		static bool _firstTime = true;
		static string _logFilePath = "Logs";

		private static void SetupLog()
		{
			_firstTime = false;

			FileStream fileStream = null;
			DirectoryInfo logDirInfo = null;
			FileInfo logFileInfo;

			_logFilePath = Path.Combine(_logFilePath, System.DateTime.Today.ToString("yyyy-MM-dd") + "." + "log");

			logFileInfo = new FileInfo(_logFilePath);
			logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
			if (!logDirInfo.Exists) logDirInfo.Create();

			if (!logFileInfo.Exists)
			{
				fileStream = logFileInfo.Create();
				fileStream.Close();
			}

			WriteLog(string.Format("\n\n{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1}", DateTime.Now, new string('-', 128)));
		}

		private static void WriteLog(string strLog)
		{
			if (_firstTime) SetupLog();

			StreamWriter log;
			FileStream fileStream = null;

			fileStream = new FileStream(_logFilePath, FileMode.Append);

			log = new StreamWriter(fileStream);
			log.WriteLine(strLog);

			log.Close();
		}
	}
}
