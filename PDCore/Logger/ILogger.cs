using System;
namespace PDCore.Logger
{
	// basic log level
	public enum Level
	{
		debug = 0,	// for debugging 
		info = 1,	// standard
		warning = 2,// warning in process ...keep a eye on it 
		error = 3,	// not good - error, exception
		silence = 4 // do not log at all
	}


    public interface ILogger
    {
        Level LogLevel { get; set; }
		void Information(string logMessage, params object?[]? args);
        void Error(string logMessage, params object?[]? args);
        void Debug(string logMessage, params object?[]? args);
        void Warning(string logMessage, params object?[]? args);
    }


}

