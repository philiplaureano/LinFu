using System;

namespace CommonServiceLocator.LinFuAdapter.Components
{
	public class AdvancedLogger : ILogger
	{
		public void Log(string msg)
		{
			Console.WriteLine("Log: {0}", msg);
		}
	}
}