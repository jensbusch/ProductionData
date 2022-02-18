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

	}
}

