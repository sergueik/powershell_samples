using System;
using CommonMark;
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
	}

}
