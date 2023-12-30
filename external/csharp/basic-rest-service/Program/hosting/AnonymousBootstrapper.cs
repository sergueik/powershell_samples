using log4net;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace ScriptServices.hosting {

    public class AnonymousBootstrapper : DefaultNancyBootstrapper {
        private readonly ConfigSettings settings;

        public AnonymousBootstrapper(ConfigSettings settings) {
            this.settings = settings;
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context) {
            pipelines.OnError.AddItemToEndOfPipeline((nancyContext, exception) =>  {
                container.Resolve<ILog>().Error("Unhandled and logged at the end of the pipeline", exception);
                return HttpStatusCode.InternalServerError;
            });
            base.RequestStartup(container, pipelines, context);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            container.Register(this.settings);
            container.Register(LogManager.Logger);

            pipelines.OnError.AddItemToEndOfPipeline((nancyContext, exception) => {              
                container.Resolve<ILog>().Error(string.Format("Unhandled and logged at the end of the pipeline"), exception);
                return HttpStatusCode.InternalServerError;
            });

            pipelines.AfterRequest += ctx => ctx.Response.WithHeader("Access-Control-Allow-Origin", "*").WithHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");

            base.ApplicationStartup(container, pipelines);
        }
    }
}