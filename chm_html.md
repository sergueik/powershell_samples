\### Expanding Archives and Folder Structures on Windows — Native, Minimal, and Historical Options



This document summarizes built-in and near-native options for extracting or expanding archives on Windows systems — without requiring Python or any non-native runtime.



It can be used to argue against proposals to introduce Python or third-party dependencies for tasks already achievable on vanilla Windows installations.



\####  — Pure PowerShell (No Invoke-Expression, No Dependencies)

Windows 10 and Later



Expand-Archive -Path "C:\\path\\to\\archive.zip" -DestinationPath "C:\\output"



Details:



Available since PowerShell 5.0 (included with Windows 10 / Server 2016+)



Handles .zip files natively



No external binaries or admin rights required



Ideal for setup scripts and automation



Windows 7–8.1 (with PowerShell 3–4)



If Expand-Archive is unavailable, use the COM-based fallback:



$shell = New-Object -ComObject Shell.Application

$zip = $shell.NameSpace("C:\\path\\to\\archive.zip")

$dest = $shell.NameSpace("C:\\output")

$dest.CopyHere($zip.Items(), 16)



Details:



Uses Windows Shell COM API (present since XP)



Runs entirely in user mode



No Invoke-Expression needed



Limitation: no progress or overwrite control



\#### Minimal External Tools

Using 7z.exe (7-Zip Command Line)



If a portable or installed 7-Zip is available:



\& "C:\\Program Files\\7-Zip\\7z.exe" x "C:\\path\\to\\archive.zip" -o"C:\\output" -y



Optional (less safe) variant:

Invoke-Expression '7z x "C:\\path\\to\\archive.zip" -o"C:\\output" -y'



Details:



Supports .zip, .7z, .tar, .gz, .iso etc.



Runs from USB or portable path



Common in enterprise and embedded setups



Using hh.exe (HTML Help Executable)



Invoke-Expression 'hh.exe mk:@MSITStore:C:\\docs\\archive.chm::/index.htm'



Details:



Opens or extracts content from Compiled HTML Help (.chm) files



Used for documentation, not general archives



Employs the mk:@MSITStore: pseudo-URL protocol



\#### — High-Overhead Alternatives (Lower Priority)

Python (for comparison only)



Installing Python just for archive extraction adds unnecessary bulk:



import zipfile

with zipfile.ZipFile("C:\\path\\to\\archive.zip", 'r') as z:

 z.extractall("C:\\output")



Reasons to avoid:



≥ 100 MB installation footprint



Internet needed for pip and updates



Maintenance burden and environment drift



No functional advantage over native tools



⚖️ Decision Summary

Method	Dependency	Works On	No iex	Formats	Notes

Expand-Archive	Built-in	Win 10+	Yes	ZIP	Modern, reliable

COM Shell	Built-in	Win 7+	Yes	ZIP	Legacy fallback

7z.exe	External	Win 7+	Yes (if using \&)	All	Fast, flexible

hh.exe	Built-in	Win 7+	No	CHM/CAB	Specialized

Python	External	All	Yes	ZIP	Overkill

\#### Appendix — CHM Structure and mk:@MSITStore Protocol



CHM Basics





.chm (Compiled HTML Help) files are compressed archives containing HTML pages, images, table of contents (.hhc), index (.hhk), and search files.





Compression uses Microsoft LZX algorithm.





Viewer binary: hh.exe.





\#### About The `mk:@MSITStore:` Protocol





A pseudo-URL scheme that lets Windows and IE components access files inside CHM archives.

Example:

mk:@MSITStore:C:\\Help\\manual.chm::/intro.htm

where mk: = Moniker protocol handler, @MSITStore: = “Microsoft Information Store” moniker, and ::/ separates internal path.





Similar “File-Like” Microsoft Schemes

SchemeExampleDescriptionres://res://shell32.dll/#24/1Accesses resources embedded in DLLs or EXEsabout:about:blankInternal IE / WebView resourcems-settings:ms-settings:privacyOpens modern Control Panel pageshell:shell:StartupOpens known system foldersfile://file:///C:/Path/True filesystem protocol

These illustrate Microsoft’s historical effort to unify “file-like” access across different storage abstractions — CHM being an early example.



\#### Takeaway

For simple archive expansion, Windows already provides PowerShell and COM-based APIs that are dependency-free and scriptable.

Adding Python or other runtimes increases maintenance, update, and policy overhead — unnecessary on constrained or locked-down systems.

