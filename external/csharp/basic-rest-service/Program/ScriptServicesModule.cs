using System;
using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.Extensions;
using Nancy.Json;
using ScriptServices.powershell;

namespace ScriptServices {
	public class ScriptServicesModule : NancyModule {
		private readonly ConfigSettings settings;

		public ScriptServicesModule(ConfigSettings settings) {
			this.settings = settings;
			JsonSettings.MaxJsonLength = 5000000;
			JsonSettings.RetainCasing = true;

			Get["/admin"] = _ =>  Response.AsJson("Hello Admin!");

			Get["/script/^(?<route>.*)$"] = p => ProcessRequest(p);

			Post["/script/^(?<route>.*)$"] = p => ProcessRequest(p);

			Put["/script/^(?<route>.*)$"] = p => ProcessRequest(p);

			Delete["/script/^(?<route>.*)$"] = p => ProcessRequest(p);
		}

		private Response ProcessRequest(dynamic routeParameters) {
			// Resolve the script being reference by the request
			var subPath = ((string)routeParameters["route"]);
			var script = string.Format("{0}.ps1", Path.Combine(this.settings.ScriptRepoRoot, subPath));
			if (!File.Exists(script)) {
				return HttpStatusCode.NotFound;
			}

			// elements of the request will be passed to the script as named routeParameters
			var scriptArgs = new Dictionary<string, string>();

			// pass querystring routeParameters
			foreach (var q in Request.Query.Keys) {
				scriptArgs.Add(q, Request.Query[q].Value);
			}

			// Process request body, if any
			var body = this.Request.Body.AsString();
			if  (!string.IsNullOrEmpty(body)) {
				scriptArgs.Add("body", body);
				Console.Error.WriteLine("Body: " + body);
			}
			Console.Error.WriteLine(String.Format("Calling: script={0} args={1}", script, scriptArgs.PrettyPrint()));
			var runner = new PowerShellRunner();
			// runner.Debug = true;
			var res = runner.Execute(Request.Method, script, scriptArgs);

			if (!res.Success) {
				return Response.AsJson(new { Error = res.Output }, HttpStatusCode.InternalServerError);
			}
			return Response.AsJson(res.Output);
		}
	}
}