using System;

namespace ApiUtilLib
{
	public class ConsoleLogger : LoggerBase
	{
		private readonly string _messageFormat = "{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1} : {2}";

        public ConsoleLogger()
        {
            LogLevel = LogLevel.None;
        }

        public ConsoleLogger(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public override void LogMessage(LogLevel messageLogLevel, string message)
        {
            Console.WriteLine(_messageFormat, DateTime.Now, messageLogLevel.ToString(), message);
        }
	}
}