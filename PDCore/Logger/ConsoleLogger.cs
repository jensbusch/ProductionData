using System;

namespace PDCore.Logger
{
    

	public class ConsoleLogger : ILogger
	{

		public Level LogLevel { get; set; } = Level.info;

		private static ConsoleLogger? _instance = null;

		public static ConsoleLogger GetInstance
		{
			get
			{
				if (_instance == null)
					_instance = new ConsoleLogger();
				return _instance;
			}
		}

		private ConsoleLogger()
		{
		}

		public void Log(Level level, string logMessage, params object?[]? args)
		{
			if(level >= LogLevel && LogLevel != Level.silence)
            {
				Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|{logMessage}", args);
            }
		}

		public void Information(string logMessage, params object?[]? args)
        {
			Log(Level.info, logMessage, args);
        }

		public void Error(string logMessage, params object?[]? args)
		{
			Log(Level.error, logMessage, args);
		}

		public void Debug(string logMessage, params object?[]? args)
		{
			Log(Level.debug, logMessage, args);
		}

		public void Warning(string logMessage, params object?[]? args)
		{
			Log(Level.warning, logMessage, args);
		}
	}
}

