# based on: https://dotnetcodr.com/2014/11/12/listing-all-performance-counters-on-windows-with-c-net/
# see also:
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcategories?view=netframework-4.5
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcounters?view=netframework-4.5

# TODO: examine if LINQ code works

add-type -TypeDefinition @'
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class PerformanceMetadataUtility {
  private List<string> categoryNames = new List<string>();
  private string categoryName = null;
  private List<string> counterNames = new List<string>();
  public String CategoryName {
    get { 
      return categoryName;
    }
    set { categoryName = value; }
  }
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
              Console.Error.WriteLine("Category: {0}, instance: {1}, counter: {2}", performanceCounter.CategoryName, instance, performanceCounter.CounterName);
              counterNames.Add(String.Format("instance: {0}, counter: {1}", instance, performanceCounter.CounterName));
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
}

'@

$o = new-object PerformanceMetadataUtility
write-output $o.CategoryNames
$o.CategoryName = 'PhysicalDisk'
write-output $o.CounterNames