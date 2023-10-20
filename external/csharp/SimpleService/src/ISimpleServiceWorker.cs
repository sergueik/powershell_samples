/// @file ISimpleServiceWorker.cs
/// @author Ron Wilson

using System;

namespace RPW.Simple
{
	interface ISimpleServiceWorker
	{
		void Init();
		void Run();
		void Cleanup();
	}
}
