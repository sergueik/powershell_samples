using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Utils
{
	public static class ResponseHelper
	{
		public static HttpResponse FromFile(this HttpResponse response, string fileName)
		{
			if (!File.Exists(fileName)) {
				response.SetContent("<html><body><h1>404 - Not Found</h1></body></html>");
				response.StatusCode = "404";
				response.Content_Type = "text/html";
				return response;
			}

			var content = File.ReadAllBytes(fileName);
			var contentType = GetMimeFromFile(fileName);
			response.SetContent(content);
			response.Content_Type = contentType;
			response.StatusCode = "200";
			return response;
		}

		public static HttpResponse FromXML(this HttpResponse response, string xmlText)
		{
			response.SetContent(xmlText);
			response.Content_Type = "text/xml";
			response.StatusCode = "200";
			return response;
		}

		public static HttpResponse FromXML<T>(this HttpResponse response, T entity) where T : class
		{
			return response.FromXML("");
		}

		public static HttpResponse FromJSON(this HttpResponse response, string jsonText)
		{
			response.SetContent(jsonText);
			response.Content_Type = "text/json";
			response.StatusCode = "200";
			return response;
		}

		public static HttpResponse FromJSON<T>(this HttpResponse response, T entity) where T : class
		{
			return response.FromJSON("");
		}

		public static HttpResponse FromText(this HttpResponse response, string text)
		{
			response.SetContent(text);
			response.Content_Type = "text/plain";
			response.StatusCode = "200";
			return response;
		}

		private static string GetMimeFromFile(string filePath) {
			string mime = "application/octet-stream";
			int MaxContent = 0;
			byte[] buf = { };
			string head = null;
			if (Type.GetType("Mono.Runtime") != null) {

				mime = MimeTypes.getMimeType(filePath);
				if (!File.Exists(filePath))
					throw new FileNotFoundException(string.Format("File {0} can't be found at server.", filePath));

				MaxContent = (int)new FileInfo(filePath).Length;
				if (MaxContent > 4096)
					MaxContent = 4096;
				buf = new byte[MaxContent];

				using (FileStream fs = File.OpenRead(filePath)) {
					fs.Read(buf, 0, MaxContent);
					fs.Close();
				}
				if (mime == "application/octet-stream" || mime == "text/plain") {
					// NOTE: Encoding.Default is locale-dependent
					head = System.Text.Encoding.ASCII.GetString(buf).TrimStart('\0', ' ', '\t', '\r', '\n').ToLowerInvariant();

					if (head.StartsWith("<!doctype html") || head.StartsWith("<html"))
						mime = "text/html";
				}
				return mime; 
			} else {
			
				IntPtr mimeout;
				if (!File.Exists(filePath))
					throw new FileNotFoundException(string.Format("File {0} can't be found at server.", filePath));

				MaxContent = (int)new FileInfo(filePath).Length;
				if (MaxContent > 4096)
					MaxContent = 4096;
				buf = new byte[MaxContent];

				using (FileStream fs = File.OpenRead(filePath)) {
					fs.Read(buf, 0, MaxContent);
					fs.Close();
				}

				int result = FindMimeFromData(IntPtr.Zero, filePath, buf, MaxContent, null, 0, out mimeout, 0);
				if (result != 0)
					throw Marshal.GetExceptionForHR(result);

				mime = Marshal.PtrToStringUni(mimeout);
				Marshal.FreeCoTaskMem(mimeout);
			}
			Console.Error.WriteLine(String.Format("Mime of {0} : {1}", filePath, mime));
            
			return mime;
		}
		// https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms775107(v=vs.85)
		// https://github.com/wine-mirror/wine/blob/71e7a2e81eec9155d5d0745b491e1cbfab3bf742/dlls/urlmon/mimefilter.c#L657
		// NOTE: the Windows FindMimeFromData does not rely on a fixed "256 bytes rule"
		// — it uses pattern-specific logic, includes security-driven sniffing rules (especially for HTML, script, etc.)
		// the Mono MimeTypes.cs is intentionally dumb/simple mapping, not detection
		// ideally one may add an critical header that is often forget
		// X-Content-Type-Options: nosniff
		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
		static extern int FindMimeFromData(IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] 
              byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)]  
              string pwzMimeProposed,
			int dwMimeFlags,
			out IntPtr ppwzMimeOut,
			int dwReserved);
	}
}