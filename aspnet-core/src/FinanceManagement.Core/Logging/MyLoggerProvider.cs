using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Logging
{
	public class MyLoggerProvider : ILoggerProvider
	{
		private Castle.Core.Logging.ILogger _logger;
		public MyLoggerProvider(Castle.Core.Logging.ILogger logger)
		{
			_logger = logger;
		}
		public ILogger CreateLogger(string categoryName)
		{
			return new MyLogger(_logger);
		}
		public void Dispose()
		{
		}
	}
}
