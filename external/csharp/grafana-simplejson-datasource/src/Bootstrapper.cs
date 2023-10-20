using Grafana.SimpleJson.Example.Common;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Grafana.SimpleJson.Example
{
    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            pipelines.EnableCORS();
        }
    }
}
