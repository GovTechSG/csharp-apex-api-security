using System;
using System.Runtime.CompilerServices;

namespace ApiUtilLib
{
    public interface ILogger
    {
        LogLevel LogLevel { get; }

        void Log(LogLevel logLevel, string message);

        void LogEnter(object[] args = null, [CallerMemberName] string caller = null);
		void LogExit(object[] args = null, [CallerMemberName] string caller = null);
		void LogEnterExit(object[] args = null, [CallerMemberName] string caller = null);

		void LogTrace(string message);
		void LogTrace(string format, params object[] args);
		
        void LogDebug(string message);
        void LogDebug(string format, params object[] args);

        void LogInformation(string message);
		void LogInformation(string format, params object[] args);
		
        void LogWarning(string message);
		void LogWarning(string format, params object[] args);
		
        void LogError(string message);
		void LogError(string format, params object[] args);
		
        void LogCritical(string message);
		void LogCritical(string format, params object[] args);
	}
}