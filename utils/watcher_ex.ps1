# https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher.changed?view=netframework-4.0
Add-Type -TypeDefinition @'
using System;
using System.IO;


public class Util {
	public static void Main() {
		var watcher = new FileSystemWatcher(@"C:\temp");

		watcher.NotifyFilter = NotifyFilters.Attributes
		| NotifyFilters.CreationTime
		| NotifyFilters.DirectoryName
		| NotifyFilters.FileName
		| NotifyFilters.LastAccess
		| NotifyFilters.LastWrite
		| NotifyFilters.Security
		| NotifyFilters.Size;

		watcher.Changed += OnChanged;
		watcher.Created += OnCreated;
		watcher.Deleted += OnDeleted;
		watcher.Renamed += OnRenamed;
		watcher.Error += OnError;

		watcher.Filter = "*.txt";
		watcher.IncludeSubdirectories = true;
		watcher.EnableRaisingEvents = true;

		Console.WriteLine("Press enter to exit.");
		Console.ReadLine();
	}

	private static void OnChanged(object sender, FileSystemEventArgs e) {
		if (e.ChangeType != WatcherChangeTypes.Changed) {
			return;
		}
		Console.WriteLine(String.Format("Changed: {0}", e.FullPath));
		if (sender != null) {
			Console.WriteLine(String.Format("Sender: {0} {1}", sender.GetType().ToString(), sender.GetHashCode().ToString()));
		}
	}

	private static void OnCreated(object sender, FileSystemEventArgs e){
		Console.WriteLine(String.Format("Created:{0}", e.FullPath));
	}

	private static void OnDeleted(object sender, FileSystemEventArgs e){	
		Console.WriteLine(String.Format("Deleted: {0}", e.FullPath));
	}
	
	private static void OnRenamed(object sender, RenamedEventArgs e){
		Console.WriteLine(String.Format("Renamed:" + "\n" + "Old: {0}" + "\n" + "New: {1}",e.OldFullPath, e.FullPath));
	}

	private static void OnError(object sender, ErrorEventArgs e){
		PrintException(e.GetException());
	}
	private static void PrintException(Exception e) {
		if (e != null) {
			Console.WriteLine(String.Format("Message: {0}" + "\n" + "Stacktrace:" + "\n" + "{1}" , e.Message), e.StackTrace);
			PrintException(e.InnerException);
		}
	}
}

'@ -ReferencedAssemblies 'System.dll'
$o  = new-object -typeName Util
[Util]::Main()
sleep -second 240

<#
$watcher =  
    [FileSystemWatcher]@{
        Path                  = $Path
        IncludeSubdirectories = $IncludeSubdirectories
        Filter                = $Filter
    }
 
Register-ObjectEvent $watcher Created -SourceIdentifier $id -MessageData $logFile -Action $OnCreated
$watcher.EnableRaisingEvents = $true
#>
