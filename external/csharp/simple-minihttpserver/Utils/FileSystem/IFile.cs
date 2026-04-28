using System;
using System.IO;

namespace MiniHttpd
{
	public interface IFile : IResource
	{
		void OnFileRequested(HttpRequest request, IDirectory directory);

		string ContentType {
			get;
		}
	}
}
