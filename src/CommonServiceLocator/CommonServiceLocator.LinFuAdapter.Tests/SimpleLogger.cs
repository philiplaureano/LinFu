using System;

namespace CommonServiceLocator.LinFuAdapter.Components
{
	public class SimpleLogger : ILogger
	{
		public void Log(string msg)
		{
			Console.WriteLine(msg);
		}
	}
}