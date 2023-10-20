/// @file SampleWorker.cs
/// @author Ron Wilson

using System;
using System.Configuration;
using System.Threading;
using RPW.Simple;

class SampleWorker : ISimpleServiceWorker
{
	private int m_frequencyMs = 60000; ///< millliseconds

	public void Init()
	{
		// initialize resources here

		try
		{
			m_frequencyMs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["frequencySeconds"]);
		}
		catch (FormatException e)
		{
			SimpleService.WriteLog(e);
		}
		catch (OverflowException e)
		{
			SimpleService.WriteLog(e);
		}

	}

	public void Run()
	{
		// perform a task here

		while (true)
		{
			SimpleService.WriteLog(string.Format("Timer Event : {0} second frequency", m_frequencyMs));
			Thread.Sleep(m_frequencyMs);
		}
	}

	public void Cleanup()
	{
		// cleanup resources here
	}
}