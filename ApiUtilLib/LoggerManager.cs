namespace ApiUtilLib
{
    public static class LoggerManager
    {
		private static LoggerBase _logger;

		public static LoggerBase Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new ConsoleLogger();
                }
                return _logger;
            }

            set => _logger = value;
        }
    }
}
