using Grafana.SimpleJson.Example.Items;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Grafana.SimpleJson.Example.Modules
{
    public class GrafanaModule : NancyModule
    {
        static int MaxResponseList = 500;        
        static List<QueryTimeserieResponse> queryResponseList = new List<QueryTimeserieResponse>();

        public GrafanaModule()
        {
            Get("/", args => {
                return new Response().WithStatusCode(HttpStatusCode.OK);
            });

            Post("/search", args =>
            {
                var searchResponse = new List<SearchResponse> { new SearchResponse { Text = "upper_25", Value = 1 }, new SearchResponse { Text = "upper_75", Value = 2 } };
                return ConvertToJson(searchResponse);
            });

            Post("/query", args =>
            {
                var request = this.Bind<QueryRequest>();
                
                //generate random traffic
                foreach (var target in request.Targets)
                {
                    if (target.Type == "timeserie")
                    {
                        var first = new QueryTimeserieResponse();

                        first.Target = target.Target;
                        first.Datapoints = new List<List<double>>
                        {
                            new List<double> { new Random().Next(30, 1000), (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds }
                        };

                        var isExistObject = queryResponseList.Find(x => x.Target == first.Target);

                        if (isExistObject != null)
                            isExistObject.Datapoints.AddRange(first.Datapoints);
                        else
                            queryResponseList.Add(first);
                    }
                    else
                    {
                        //process table format here
                    }
                }

                //clear data
                if (queryResponseList.Count > MaxResponseList)
                    queryResponseList.Clear();

                return ConvertToJson(queryResponseList);
            });

            Post("/annotations", args =>
            {
                var request = this.Bind<AnatationRequest>();

                //process annotations here.

                return string.Empty;
            });

            Post("/tag-keys", args =>
            {
                var tags = new TagKey[2];
                tags[0] = new TagKey { Type = "string", Text = "City" };
                tags[1] = new TagKey { Type = "string", Text = "Country" };

                return JsonConvert.SerializeObject(tags);
            });

            Post("/tag-values", args =>
            {
                var tags = new TagKey[2];
                tags[0] = new TagKey { Type = "string", Text = "City" };
                tags[1] = new TagKey { Type = "string", Text = "Country" };

                return JsonConvert.SerializeObject(tags);
            });
        }

        private string ConvertToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return json;
        }
    }
}

