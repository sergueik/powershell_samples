using System;
using Markdig;
using System.IO;

namespace Utils {

	public class MarkdownConverter : IMarkdownConverter  {
		// Includes Table + many more
		private   MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

		public string convert() {
			string payload = File.ReadAllText("README.md");
			var result = Markdown.ToHtml(payload, pipeline);
			return result;
		}

		public string convert(string payload) {
			var result = Markdown.ToHtml(payload, pipeline);
			return result;
		}
		
		public string convertFile(string filePath) {
        var content = File.ReadAllText(filePath);
        return this.convert(content);
    }
	}
	
	

}
