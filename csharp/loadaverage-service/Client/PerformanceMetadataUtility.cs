using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

// based on: https://dotnetcodr.com/2014/11/12/listing-all-performance-counters-on-windows-with-c-net/
// see also:
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcategories?view=netframework-4.5
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcounters?view=netframework-4.5
namespace TransactionService
{
	
	public class PerformanceMetadataUtility
	{
		private string categoryName = null;
		public String CategoryName {
			get { 
				return categoryName;
			}
			set { categoryName = value; }
		}
		private List<string> categoryNames = new List<string>();
		public List<string> CategoryNames {
			get {
				if (categoryNames.Count == 0) {
					var categories = PerformanceCounterCategory.GetCategories();
					foreach (PerformanceCounterCategory performanceCounterCategory in categories) {
						categoryNames.Add(performanceCounterCategory.CategoryName);
					}
				}
				return categoryNames;
			}
		}
		private List<string> counterNames = new List<string>();
		public List<string> CounterNames {
			get {
				if (categoryName != null) {
					var performanceCounterCategory = new PerformanceCounterCategory(categoryName);
					
					var instances = performanceCounterCategory.GetInstanceNames();
					if (instances.Any()) {
						var instance = instances.First();
						if (performanceCounterCategory.InstanceExists(instance)) {
							var counters = performanceCounterCategory.GetCounters(instance);
							foreach (PerformanceCounter performanceCounter in counters) {
								// Console.WriteLine("Category: {0}, instance: {1}, counter: {2}", performanceCounter.CategoryName, instance, performanceCounter.CounterName);
								// TODO: find the appropriate format for instance
								counterNames.Add(performanceCounter.CounterName);
							}
						}
					} else {
						var counters = performanceCounterCategory.GetCounters();
						foreach (PerformanceCounter performanceCounter in counters) {
							counterNames.Add(performanceCounter.CounterName);
						}
					}
				}
				return counterNames;
				
			}
		}
		private string counterName = null;
		public string CounterName {
			get { 
				return counterName;
			}
			set { counterName = value; }
		}
	
		// private readonly bool valid = false;
		public Boolean Valid {
			get { 
				if (this.CategoryName.Length == 0 || this.CounterName.Length == 0) {
					return false;
				}
				var performanceCounterCategory = new PerformanceCounterCategory(categoryName);
				var instances = performanceCounterCategory.GetInstanceNames();
				if (instances.Any()) {
					// System.ArgumentException: Counter is not single instance, an instance name needs to be specified.
					foreach (string instance in instances) {
						if (performanceCounterCategory.InstanceExists(instance)) {
							var counters = performanceCounterCategory.GetCounters(instance);
							foreach (PerformanceCounter performanceCounter in counters) {
								if (performanceCounter.CounterName.Equals(this.CounterName))
									return true;
							}
						}
					}
				} else {
					var counters = performanceCounterCategory.GetCounters();
					foreach (PerformanceCounter performanceCounter in counters) {
						if (performanceCounter.CounterName.Equals(this.CounterName))
							return true;
					}   
				}
				return false;
			}
			
		}
		
	}
}
