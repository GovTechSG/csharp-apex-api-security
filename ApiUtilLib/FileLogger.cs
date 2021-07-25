using System;
using System.IO;

namespace ApiUtilLib
{
	public class FileLogger : LoggerBase
	{
		private readonly string _messageFormat = "{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1} : {2}";

		public FileLogger()
		{
			LogLevel = LogLevel.None;
		}

		public FileLogger(LogLevel logLevel)
		{
			LogLevel = logLevel;
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

		private static bool _firstTime = true;
		private static string _logFilePath = "Logs";

		private static void SetupLog()
		{
			_firstTime = false;
            FileInfo logFileInfo;

			_logFilePath = Path.Combine(_logFilePath, DateTime.Today.ToString("yyyy-MM-dd") + "." + "log");

			logFileInfo = new FileInfo(_logFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists)
            {
                logDirInfo.Create();
            }

            if (!logFileInfo.Exists)
			{
                FileStream fileStream = logFileInfo.Create();
                fileStream.Close();
			}

			WriteLog(string.Format("\n\n{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1}", DateTime.Now, new string('-', 128)));
		}

		private static void WriteLog(string strLog)
		{
			if (_firstTime)
            {
                SetupLog();
            }

            StreamWriter log;
            FileStream fileStream = new FileStream(_logFilePath, FileMode.Append);

            log = new StreamWriter(fileStream);
			log.WriteLine(strLog);

			log.Close();
		}
	}
}
