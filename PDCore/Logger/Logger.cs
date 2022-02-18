using System;
namespace mqtt
{
    public enum Level
    {
		debug = 0,
		info = 1,
		warning = 2,
		error = 3,
		silence = 4
    }

	public class Logger
	{
		string _name = "module";

		public Level LogLevel { get; set; } = Level.info;

		private static Logger? _instance = null;

		public static Logger GetInstance
		{
			get
			{
				if (_instance == null)
					_instance = new Logger();
				return _instance;
			}
		}

		private Logger()
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

