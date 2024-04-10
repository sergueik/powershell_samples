// based on: https://dotnetcodr.com/2014/11/12/listing-all-performance-counters-on-windows-with-c-net/
// see also:
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcategories?view=netframework-4.5
// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory.getcounters?view=netframework-4.5
PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();
foreach (PerformanceCounterCategory category in categories)
{
    Console.WriteLine("Category name: {0}", category.CategoryName);
    Console.WriteLine("Category type: {0}", category.CategoryType);
    Console.WriteLine("Category help: {0}", category.CategoryHelp);
    string[] instances = category.GetInstanceNames();
    if (instances.Any())
    {
        foreach (string instance in instances)
        {
            if (category.InstanceExists(instance))
            {
                PerformanceCounter[] countersOfCategory = category.GetCounters(instance);
                foreach (PerformanceCounter pc in countersOfCategory)
                {
                    Console.WriteLine("Category: {0}, instance: {1}, counter: {2}", pc.CategoryName, instance, pc.CounterName);
                }
            }
        }
    }
    else
    {
        PerformanceCounter[] countersOfCategory = category.GetCounters();
        foreach (PerformanceCounter pc in countersOfCategory)
        {
                    Console.WriteLine("Category: {0}, counter: {1}", pc.CategoryName, pc.CounterName);
        }
    }   
}
