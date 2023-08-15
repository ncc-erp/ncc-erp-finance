using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Logging
{
	public class MyLogger : ILogger
	{
		private Castle.Core.Logging.ILogger _logger;
		public MyLogger(Castle.Core.Logging.ILogger logger)
		{
			_logger = logger;
		}
		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}
		public void Log<TState>(LogLevel logLevel, EventId eventId,
			TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (IsEnabled(logLevel))
			{
				var msg = formatter(state, exception);
				//_logger.Info("DB-REQUEST: " + msg);
				Console.WriteLine("DB-REQUEST: \r" +msg);
			}
		}
	}
}
