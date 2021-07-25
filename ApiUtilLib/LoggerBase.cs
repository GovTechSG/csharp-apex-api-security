using System;
//using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ApiUtilLib
{
    public abstract class LoggerBase : ILogger
	{
		protected LogLevel _logLevel = LogLevel.None;

        protected LoggerBase __child;

        public void SetChainLogger(LoggerBase childLogger)
        {
            if (__child != null)
            {
                throw new ArgumentException("Chain logger already been set.");
            }

            __child = childLogger;
        }

		public LogLevel LogLevel
		{
			get { return _logLevel; }
			set { _logLevel = value; }
		}

        void ILogger.Log(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                    break;
                case LogLevel.Trace:
                    LogTrace(message);
                    break;
                case LogLevel.Debug:
                    LogDebug(message);
                    break;
                case LogLevel.Information:
                    LogInformation(message);
                    break;
                case LogLevel.Warning:
                    LogWarning(message);
                    break;
                case LogLevel.Error:
                    LogError(message);
                    break;
                case LogLevel.Critical:
                    LogCritical(message);
                    break;
            }
        }

        public static object[] Args(params object[] args)
		{
			return args;
		}

		public void LogEnterExit(object[] args = null, [CallerMemberName] string caller = null)
		{
			LogMethodCall("{0} Enter/Exit :: Params :: {1}", args, caller);
		}

		public void LogEnter(object[] args = null, [CallerMemberName] string caller = null)
		{
			LogMethodCall("{0} Enter :: Params :: {1}", args, caller);
		}

		public void LogExit(object[] args = null, [CallerMemberName] string caller = null)
		{
			LogMethodCall("{0} Exit :: Return :: {1}", args, caller);
		}

		private void LogMethodCall(string format, object[] args = null, string caller = null)
		{
			string argsList = "";
			string delimiter = "";

			foreach (object item in args)
			{
				if (item == null)
				{
					argsList += string.Format("{0}null", delimiter);
				}
				else
				{
					argsList += string.Format("{0}\"{1}\"", delimiter, item);
				}

				delimiter = ", ";
			}

			if (string.IsNullOrEmpty(argsList))
            {
                argsList = "none";
            }

            LogTrace(string.Format(format, caller, argsList));
		}

		public void LogCritical(string message)
		{
			InternalLogMessage(LogLevel.Critical, message);
		}

		public void LogCritical(string format, params object[] args)
		{
			LogCritical(string.Format(format, args));
		}

		public void LogDebug(string message)
		{
			InternalLogMessage(LogLevel.Debug, message);
		}

		public void LogDebug(string format, params object[] args)
		{
			LogDebug(string.Format(format, args));
		}

		public void LogError(string message)
		{
			InternalLogMessage(LogLevel.Error, message);
		}

		public void LogError(string format, params object[] args)
		{
			LogError(string.Format(format, args));
		}

		public void LogInformation(string message)
		{
			InternalLogMessage(LogLevel.Information, message);
		}

		public void LogInformation(string format, params object[] args)
		{
			LogInformation(string.Format(format, args));
		}

		public void LogTrace(string message)
		{
			InternalLogMessage(LogLevel.Trace, message);
		}

		public void LogTrace(string format, params object[] args)
		{
			LogTrace(string.Format(format, args));
		}

		public void LogWarning(string message)
		{
			InternalLogMessage(LogLevel.Warning, message);
		}

		public void LogWarning(string format, params object[] args)
		{
			LogWarning(string.Format(format, args));
		}

        private void InternalLogMessage(LogLevel messageLogLevel, string message)
		{
            if (_logLevel <= messageLogLevel)
            {
                LogMessage(messageLogLevel, message);
            }

            if (__child != null)
            {
                __child.InternalLogMessage(messageLogLevel, message);
            }
        }

        public abstract void LogMessage(LogLevel messageLogLevel, string message);
    }
}