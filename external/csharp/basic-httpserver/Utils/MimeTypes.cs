﻿using System;
using System.Collections.Generic;
using System.Linq;

// origin:
// https://github.com/samuelneff/MimeTypeMap/blob/master/MimeTypeMap.cs
// https://github.com/mono/mono/blob/main/mcs/class/System.Web/System.Web/MimeTypes.cs
// see also:
// https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types/Common_types
namespace Utils {

	public static class MimeTypes {
		    
		private static readonly Lazy<IDictionary<string, string>> mappings = new Lazy<IDictionary<string, string>>(buildMappings);
		// Identifier expected; 'default' is a keyword (CS1041)
		// private const string default = "application/octet-stream";

		public static bool tryGetMimeType(string str, out string mimeType) {
            if (str == null) {
                throw new ArgumentNullException("invalid argument");
            }
			int index = str.LastIndexOf(".");
            if (index != -1 && str.Length > index + 1) {
                str = str.Substring(index + 1);
            }
       		str = "." + str;

            return mappings.Value.TryGetValue(str, out mimeType);
        }
        public static string getMimeType(string str) {
			var result = "";
            return MimeTypes.tryGetMimeType(str, out result) ? result : "application/octet-stream";
        }
		private static IDictionary<string, string> buildMappings() {
			 return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
				{ ".7z", "application/x-7z-compressed" },
                { ".aspx", "text/html" },
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".c", "text/plain"},
                {".cab", "application/octet-stream"},
                {".chm", "application/octet-stream"},
                {".class", "application/x-java-applet"},
                {".cmd", "text/plain"},
                {".cnf", "text/plain"},
                {".config", "application/xml"},
                {".cpp", "text/plain"},
                {".cs", "text/plain"},
                {".csproj", "text/plain"},
                {".css", "text/css"},
                {".dat", "application/octet-stream"},
                {".dll", "application/octet-stream"},
                {".dll.config", "text/xml"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".exe", "application/octet-stream"},
                {".exe.config", "text/xml"},
                {".flac", "audio/flac"},
                {".gif", "image/gif"},
                {".gz", "application/x-gzip"},
                {".hhc", "application/x-oleobject"},
                {".hta", "application/hta"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".ini", "text/plain"},
                {".jar", "application/java-archive"},
                {".java", "application/octet-stream"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/javascript"},
                {".json", "application/json"},
                {".midi", "audio/mid"},
                {".mov", "video/quicktime"},
                {".mp4", "video/mp4"},
                {".mpeg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".pdf", "application/pdf"},
                {".png", "image/png"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".rar", "application/x-rar-compressed"},
                {".resx", "application/xml"},
                {".rtf", "application/rtf"},
                {".sit", "application/x-stuffit"},
                {".sln", "text/plain"},
                {".sql", "text/plain"},
                {".tiff", "image/tiff"},
                {".ttf", "application/font-sfnt"},
                {".txt", "text/plain"},
                {".vb", "text/plain"},
                {".vbproj", "text/plain"},
                {".vbs", "text/vbscript"},
                {".vsix", "application/vsix"},
                {".vsixmanifest", "text/xml"},
                {".wav", "audio/wav"},
                {".webm", "video/webm"},
                {".woff", "application/font-woff"},
                {".woff2", "application/font-woff2"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xml", "text/xml"},
                {".yml", "text/yaml"},
                {".yaml", "text/yaml"},
                {".z", "application/x-compress"},
                {".zip", "application/zip"}
        	};
		}
	}
}
