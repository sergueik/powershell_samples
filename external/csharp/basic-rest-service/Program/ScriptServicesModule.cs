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

		// Dynamic binding defers the process of resolving types, members, and operations from compile time to runtime
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-dynamic-type
		public ScriptServicesModule(ConfigSettings settings) {
			this.settings = settings;
			JsonSettings.MaxJsonLength = 5000000;
			JsonSettings.RetainCasing = true;

			Get["/admin"] = _ =>  Response.AsJson("Hello Admin!");

			Get["/script/^(?<scriptBaseName>.*)$"] = (dynamic scriptParameters) => ProcessRequest(scriptParameters);

			Post["/script/^(?<scriptBaseName>.*)$"] = (dynamic scriptParameters) => ProcessRequest(scriptParameters);

			Put["/script/^(?<scriptBaseName>.*)$"] = (dynamic scriptParameters) => ProcessRequest(scriptParameters);

			Delete["/script/^(?<scriptBaseName>.*)$"] = (dynamic scriptParameters) => ProcessRequest(scriptParameters);
		}

		private Response ProcessRequest(dynamic scriptParameters) {
			// Resolve the script being reference by the request


			var scriptBaseName = ((string)scriptParameters["scriptBaseName"]);
			var script = string.Format("{0}.ps1", Path.Combine(this.settings.ScriptRepoRoot, scriptBaseName));
			if (!File.Exists(script)) {
				return HttpStatusCode.MethodNotAllowed;
			}

			// NOTE: cannot use "as" syntax:
			// var scriptBaseName = scriptParameters["scriptBaseName"] as string;
			// Something went horribly, horribly wrong while servicing your request

			var args = new Dictionary<string, string>();

			foreach (var queryKey in Request.Query.Keys) {
				args.Add(queryKey, Request.Query[queryKey].Value);
			}
			var body = this.Request.Body.AsString();
			if  (!string.IsNullOrEmpty(body)) {
				args.Add("body", body);
				// Console.Error.WriteLine("Body: " + body);
			}
			// Console.Error.WriteLine(String.Format("Calling: script={0} args={1}", script, args.PrettyPrint()));
			var runner = new PowerShellRunner();
			// runner.Debug = true;
			var response = runner.Execute(Request.Method, script, args);
			return response.Success ? Response.AsJson(response.Output) :Response.AsJson(new { Error = response.Output }, HttpStatusCode.InternalServerError);
		}
	}
}
