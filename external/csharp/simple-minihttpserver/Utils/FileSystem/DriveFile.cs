using System;
using System.IO;
using System.Runtime.Serialization;

namespace MiniHttpd
{
	[Serializable]
	public class DriveFile : IFile, IPhysicalResource
	{
		public DriveFile(string path, IDirectory parent)
			: this(path, parent, true)
		{
		}

		internal DriveFile(string path, IDirectory parent, bool checkPath)
		{
			if (checkPath) {
				path = System.IO.Path.GetFullPath(path);
				if (!File.Exists(path))
					throw new FileNotFoundException(path + " not found");
				if (path.IndexOfAny(System.IO.Path.InvalidPathChars) >= 0)
					throw new ArgumentException("Path cantains invalid characters.", "path");
			}
			this.path = path;
			this.parent = parent;
		}

		string path;
		string name;
		IDirectory parent;

		public string Path {
			get {
				return path;
			}
		}

		#region IFile Members

		public void OnFileRequested(HttpRequest request, IDirectory directory)
		{
			if (request.IfModifiedSince != DateTime.MinValue) {
				if (File.GetLastWriteTimeUtc(path) < request.IfModifiedSince)
					request.Response.ResponseCode = "304";
				return;
			}
			if (request.IfUnmodifiedSince != DateTime.MinValue) {
				if (File.GetLastWriteTimeUtc(path) > request.IfUnmodifiedSince)
					request.Response.ResponseCode = "304";
				return;
			}

			if (System.IO.Path.GetFileName(path).StartsWith(".")) {
				request.Response.ResponseCode = "403";
				return;
			}

			try {
				request.Response.ResponseContent = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			} catch (FileNotFoundException) {
				request.Response.ResponseCode = "404";
			} catch (IOException e) {
				request.Response.ResponseCode = "500";
				request.Server.Log.WriteLine(e);
			}
		}

		public string ContentType {
			get {
				return ContentTypes.GetExtensionType(System.IO.Path.GetExtension(path));
			}
		}

		public string Name {
			get {
				if (parent == null)
					return null;
				if (name == null)
					name = System.IO.Path.GetFileName(path);
				return name;
			}
		}

		public IDirectory Parent {
			get {
				return parent;
			}
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion

		public override string ToString()
		{
			return name;
		}
	}
}

