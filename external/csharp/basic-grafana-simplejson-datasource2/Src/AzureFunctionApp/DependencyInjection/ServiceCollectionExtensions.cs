using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GrafanaSimpleDataSource.Data.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataSources(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Just return if we've already added Data source collection to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(Dictionary<string, IDataSource>)))
                return services;

            var dataSourcesCollection = new Dictionary<string, IDataSource>();

            var assembliesToScanArray = assemblies as Assembly[] ?? assemblies?.ToArray();

            if (assembliesToScanArray != null && assembliesToScanArray.Length > 0)
            {
                var allTypes = assembliesToScanArray
                    .Where(a => !a.IsDynamic)
                    .Distinct() 
                    .SelectMany(a => a.DefinedTypes)
                    .ToArray();

                var dataSourceTypes = new[]
                {
                    typeof(IDataSource),
                };
                var allDataSourceTypes = dataSourceTypes.SelectMany(openType => allTypes
                    .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.AsType().ImplementsInterface(openType))).ToList();
                foreach (var type in allDataSourceTypes)
                {
                    // use try add to avoid double-registration
                    Type t = type.AsType();
                    services.TryAddTransient(t);

                    // Create an instance in the data source collection
                    IDataSource obj = (IDataSource)Activator.CreateInstance(t);
                    dataSourcesCollection.Add(obj.Metric, obj);
                }
            }

            services.AddScoped(s => dataSourcesCollection);

            return services;
        }

        private static bool ImplementsInterface(this Type type, Type interfaceType)
            => type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsAssignableFrom(interfaceType));
    }
}
