using System;

namespace ApiUtilLib
{
	public class ConsoleLogger : LoggerBase
	{
		readonly string _messageFormat = "{0:yyyy-MM-dd HH:mm:ss.fff %K} : {1} : {2}";

		public ConsoleLogger()
		{
			this.LogLevel = LogLevel.None;
		}

        public ConsoleLogger(LogLevel logLevel)
		{
			this.LogLevel = logLevel;
		}

        public override void LogMessage(LogLevel messageLogLevel, string message)
        {
            Console.WriteLine(_messageFormat, DateTime.Now, messageLogLevel.ToString(), message);
        }
	}
}