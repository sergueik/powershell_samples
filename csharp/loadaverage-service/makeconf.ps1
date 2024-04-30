#Copyright (c) 2024 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# based on: https://dotnetcodr.com/2014/11/12/listing-all-performance-counters-on-windows-with-c-net/
# see also:
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcategories?view=netframework-4.5
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcounters?view=netframework-4.5

param(
  [string]$category = '', # 'PhysicalDisk'
  [string]$counter = '',
  [switch]$configure,
  [switch]$list,
  [switch]$help,
  [switch]$debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()

add-type -TypeDefinition @'

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
'@

if ([bool]$psboundparameters['help'].ispresent) {

  if ($category -ne '') {
    if ($counter -ne '') {

      $o = new-object PerformanceMetadataUtility
      $o.CategoryName = $category
      $o.CounterName = $counter
      if ($o.Valid) {
        write-output $o.CounterHelp 
      } else {
        write-output ('The combination of Category "{0}" and Counter "{1}" is Invalid' -f $category,$counter)
      }
      exit
    }
  }


$name = 'makeconf.ps1'
  write-host @"
Example Usage:
${name} [-list]
examine performance counters

Options:

list       - list list Performance Categories or list Performance Counters for specific Category, if one is provided
configure - print the application config section
category  - name of the Category to explore e.g. 'PhysicalDisk'
NOTE: some categories also require an intance to return the counters (see example below)
counter   - name of the Counter to write config e.g. . When none is provided, list Counters for specified category are listed
force
help
debug
Example Usage:
.\${name} -list
prints  some 100 category names
.\${name} -category 'PhysicalDisk' -list
prints some 20 counter names belonging to 'PhysicalDisk' category
.\${name} -list -Category Memory

.\${name} -Category Memory -Counter 'Available bytes'
prints app.xml fragment:
    <add key="CategoryName" value="Memory"/>
    <add key="CounterName" value="Available bytes"/>
    <add key="InstanceName" value=""/>

.\${name} -Category PhysicalDisk -Counter '% Disk Time'
prints app.xml fragment:

<add key="CategoryName" value="PhysicalDisk"/>
<add key="CounterName" value="% Disk Time"/>
<add key="InstanceName" value="0 C: D:"/>

.\${name} -Category X -Counter Y
will print an error
The combination of Category X and Counter Y is Invalid

.\${name} -help -Category PhysicalDisk -Counter '% Disk Time'
% Disk Time is the percentage of elapsed time that the selected disk drive was busy servicing read or write requests.

"@
  exit
}


$o = new-object PerformanceMetadataUtility
if ([bool]$psboundparameters['list'].ispresent) {
  if ($category -eq '') {
    write-output $o.CategoryNames
  } else {
    $o.CategoryName = $category
    write-output $o.CounterNames
  }
} else {
  
  # Perform check explicitly
  if ($category -ne '') {
    if ($counter -ne '') {
      $o.CategoryName = $category
      
      $o.CounterName = $counter  
      # NOTE: 'Valid' is a getter
      if ($o.Valid) {
	 # TODO: while verifying that a category / counter exist, provide instance hint
      	 $instance = $o.InstanceName;
      	 write-output @"
      	<add key="CategoryName" value="${category}"/>
      	<add key="CounterName" value="${counter}"/>
      	<add key="InstanceName" value="${instance}"/>
"@
      } else {
        write-output ('The combination of Category "{0}" and Counter "{1}" is Invalid' -f $category,$counter)
      }
      
   }  
 }
}
