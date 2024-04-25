using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

public class PerformanceMetadataUtility {

  private string categoryName = null;
  private string instanceName = null;
  private List<string> categoryNames = new List<string>();
  private List<string> counterNames = new List<string>();
  private string counterName = null;
  // private readonly bool valid = false;

  public String InstanceName {
    get {
      return instanceName;
    }
  }
  public String CategoryName {
    get {
      return categoryName;
    }
    set { categoryName = value; }
  }
  public List<string> CategoryNames {
    get {
      if (categoryNames.Count == 0) {
  		// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory?view=netframework-4.5
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
              // Console.WriteLine("Category: {0}, instance: {1}, counter: {2}", performanceCounter.CategoryName, instance, performanceCounter.CounterName);
              // TODO: find the appropriate format for instance
              counterNames.Add(performanceCounter.CounterName);
            }
          }
        } else {
          var counters = performanceCounterCategory.GetCounters();
          // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter?view=netframework-4.5
          foreach (PerformanceCounter performanceCounter in counters) {
            counterNames.Add(performanceCounter.CounterName);
          }
        }
      }
      return counterNames;

    }
  }

  public string CounterName {
    get {
      return counterName;
    }
    set { counterName = value; }
  }

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
		  // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.instanceexists?view=netframework-4.5#system-diagnostics-performancecountercategory-instanceexists(system-strings)
          if (performanceCounterCategory.InstanceExists(instance)) {
            var counters = performanceCounterCategory.GetCounters(instance);
            foreach (PerformanceCounter performanceCounter in counters) {
              if (performanceCounter.CounterName.Equals(this.CounterName)) {
                this.instanceName = instance;
                return true;
              }
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


  public String CounterHelp {
    get {
      if (this.CategoryName.Length == 0 || this.CounterName.Length == 0) {
        return null;
      }

      var performanceCounterCategory = new PerformanceCounterCategory(categoryName);
      var instances = performanceCounterCategory.GetInstanceNames();
      if (instances.Any()) {
        // System.ArgumentException: Counter is not single instance, an instance name needs to be specified.
        foreach (string instance in instances) {
		  // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.instanceexists?view=netframework-4.5#system-diagnostics-performancecountercategory-instanceexists(system-strings)
          if (performanceCounterCategory.InstanceExists(instance)) {
            var counters = performanceCounterCategory.GetCounters(instance);
            foreach (PerformanceCounter performanceCounter in counters) {
              if (performanceCounter.CounterName.Equals(this.CounterName)) {
                // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter.counterhelp?view=netframework-4.5
                return performanceCounter.CounterHelp;
              }
            }
          }
        }
      } else {
        var counters = performanceCounterCategory.GetCounters();
        foreach (PerformanceCounter performanceCounter in counters) {
          if (performanceCounter.CounterName.Equals(this.CounterName))
          	// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter.counterhelp?view=netframework-4.5            		
            return performanceCounter.CounterHelp;
        }
      }
      return null;
    }
  }

}
