using System;
using CommonMark;
using Markdig;
using System.IO;

namespace Utils {

	public class MarkdownConvertor {

		public string convert() {
			string payload = File.ReadAllText("README.md");
			var result = CommonMarkConverter.Convert(payload);
			return result;
		}

		public string convert(string payload) {
			var result = CommonMarkConverter.Convert(payload);
			return result;
		}
		
		public string convert2(string payload) {
			var result = Markdown.ToHtml(payload);
			return result;
		}
	}

}
