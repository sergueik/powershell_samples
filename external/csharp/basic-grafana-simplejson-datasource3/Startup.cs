using System;
using System.Threading.Tasks;
using GrafanaGenericSimpleJsonDataSource.Converters;
using GrafanaGenericSimpleJsonDataSource.Data;
using GrafanaGenericSimpleJsonDataSource.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof(GrafanaGenericSimpleJsonDataSource.Startup))]

namespace GrafanaGenericSimpleJsonDataSource
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(
            options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter(true));
                options.SerializerSettings.Converters.Add(new CSVDataJsonConverter());
                options.SerializerSettings.Converters.Add(DataPointConverter<byte>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<sbyte>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<short>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<ushort>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<int>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<uint>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<long>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<ulong>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<float>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<double>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<string>.Instance);
                options.SerializerSettings.Converters.Add(DataPointConverter<CsvData>.Instance);
            });
            RepositoryFactory factory = new RepositoryFactory();
            services.AddSingleton<RepositoryFactory>(factory);
            factory.CreateRepository<CsvData>("SampleCsvRepo", nameof(CsvRepository));
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMvc();
            applicationBuilder.UseMvcWithDefaultRoute();
        }
    }
}
