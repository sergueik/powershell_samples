using System;
using System.Collections;
using System.IO;

namespace MiniHttpd
{
	public class IndexPage : IFile
	{

		#region IFile Members
		public string ContentType {
			get {
				return ContentTypes.GetExtensionType(".htm");
			}
		}

		internal virtual void PrintBody(StreamWriter writer,
			HttpRequest request,
			IDirectory directory,
			ICollection dirs,
			ICollection files
		)
		{

			writer.WriteLine("<h2>Index of " + HttpWebServer.GetDirectoryPath(directory) + "</h2>");

			if (directory.Parent != null)
				writer.WriteLine("<a href=\"..\">[..]</a><br>");

			foreach (IDirectory dir in dirs) {
				//if(dir is IPhysicalResource)
				//	if((File.GetAttributes((dir as IPhysicalResource).Path) & FileAttributes.Hidden) != 0)
				//		continue;

				writer.WriteLine("<a href=\"" + UrlEncoding.Encode(dir.Name) + "/\">[" + dir.Name + "]</a><br>");
			}

			foreach (IFile file in files) {
				//if(file is IPhysicalResource)
				//	if((File.GetAttributes((file as IPhysicalResource).Path) & FileAttributes.Hidden) != 0)
				//		continue;
				writer.WriteLine("<a href=\"" + UrlEncoding.Encode(file.Name) + "\">" + file.Name + "</a><br>");
			}
		}

		public void OnFileRequested(HttpRequest request, IDirectory directory)
		{
			ICollection dirs;
			ICollection files;
			try {
				dirs = directory.GetDirectories();
				files = directory.GetFiles();
			} catch (UnauthorizedAccessException) {
				throw new HttpRequestException("403");
			}

			request.Response.BeginChunkedOutput();
			StreamWriter writer = new StreamWriter(request.Response.ResponseContent);

			writer.WriteLine("<html>");
			writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
			writer.WriteLine("<head><title>Index of " + HttpWebServer.GetDirectoryPath(directory) + "</title></head>");
			writer.WriteLine("<body>");

			PrintBody(writer, request, directory, dirs, files);
			
			writer.WriteLine("<hr>" + request.Server.ServerName);
			writer.WriteLine("</body></html>");

			writer.WriteLine("</body>");
			writer.WriteLine("</html>");
			writer.Flush();
		}

		public string Name {
			get {
				return null;
			}
		}

		public IDirectory Parent {
			get {
				return null;
			}
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion
	}
}
