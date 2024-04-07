using System;
using System.Diagnostics;
using System.Linq;

// based on: https://dotnetcodr.com/2014/11/12/listing-all-performance-counters-on-windows-with-c-net/
// see also:
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcategories?view=netframework-4.5
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcounters?view=netframework-4.5
namespace TransactionService  {
	
	public class Utility {
		private string[] categoryNames = { };
		private string categoryName = null;
		private string[] counterNames = { };
		public String CategoryName {
			get { 
				return categoryName;
			}
			set { categoryName = value; }
		}
		public String[] CategoryNames {
			get {
				if (categoryNames.Length == 0) {
					var categories = PerformanceCounterCategory.GetCategories();
					foreach (PerformanceCounterCategory performanceCounterCategory in categories) {
						categoryNames.Append(performanceCounterCategory.CategoryName);
					}
				}
				return categoryNames;
			}
		}
		public String[] CounterNames {
			get {
				if (categoryName != null) {
					var performanceCounterCategory = new PerformanceCounterCategory(categoryName);
					var counters = performanceCounterCategory.GetCounters();
					foreach (PerformanceCounter performanceCounter in counters) {
						counterNames.Append(performanceCounter.CounterName);
					}
				}
				return counterNames;
				
			}
		}
		public void load()
		{
			var categories = PerformanceCounterCategory.GetCategories();
			foreach (PerformanceCounterCategory performanceCounterCategory in categories) {
				Console.WriteLine("Category name: {0}", performanceCounterCategory.CategoryName);
				Console.WriteLine("Category type: {0}", performanceCounterCategory.CategoryType);
				Console.WriteLine("Category help: {0}", performanceCounterCategory.CategoryHelp);
				var instances = performanceCounterCategory.GetInstanceNames();
				if (instances.Any()) {
					foreach (string instance in instances) {
						if (performanceCounterCategory.InstanceExists(instance)) {
							var counters = performanceCounterCategory.GetCounters(instance);
							foreach (PerformanceCounter performanceCounter in counters) {
								Console.WriteLine("Category: {0}, instance: {1}, counter: {2}", performanceCounter.CategoryName, instance, performanceCounter.CounterName);
							}
						}
					}
				} else {
					var counters = performanceCounterCategory.GetCounters();
					foreach (PerformanceCounter performanceCounter in counters) {
						Console.WriteLine("Category: {0}, counter: {1}", performanceCounter.CategoryName, performanceCounter.CounterName);
					}
				}   
			}

		}
	}
}
