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
  [switch]$all,
  [switch]$help,
  [switch]$debug
)

if ([bool]$psboundparameters['help'].ispresent) {
  write-host @"
Example Usage:
perf_counter_config.ps1 [-all]
examine performanc counters

Options:

all       - list all Performance Categories or all Performance Counters for specific Category, if one is provided
configure - print the application config section
category  - name of the Category to explore e.g. 'PhysicalDisk'
NOTE: some categories also require an intance to return the counters (see example below)
counter   - name of the Counter to write config e.g. . When none is provided, all Counters for specified category are listed
force
help
debug
Example Usage:

.\perf_counter_config.ps1 -category 'PhysicalDisk' -all
.\perf_counter_config.ps1 -all -Category Memory

.\perf_counter_config.ps1  -Category Memory -Counter 'Available bytes'
"@
  exit
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
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
if ([bool]$psboundparameters['all'].ispresent) {
  if ($category -eq '') {
    write-output $o.CategoryNames
  } else {
    $o.CategoryName = $category
    write-output $o.CounterNames
  }
} else {
  # TODO: verify that a category / counter exist, provide instance hint
  if ($category -ne '') {
    if ($counter -ne '') {
     $instance = '';
     write-output @"
    <add key="CategoryName" value="${category}"/>
    <add key="CounterName" value="${counter}"/>
    <add key="InstanceName" value="${instance}"/>

"@
   }  
 }
}
