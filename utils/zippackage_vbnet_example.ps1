#Copyright (c) 2023 Serguei Kouzmine
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# origin: https://www.codeproject.com/Articles/28107/Zip-Files-Easy
param (
  [string]$archive = 'C:\temp\dummy.zip',
  [string]$filepath = 'C:\temp\a.txt'
)
Add-Type -Language 'VisualBasic' -TypeDefinition @"

Imports System.IO.Packaging
Imports System.IO
Imports System
Public Class Example
Dim fileToAdd As String = "C:\TEMP\Compression\Compress Me.txt"
    Public Sub New(ByVal argZipPath As System.String,ByVal argfileToAdd As System.String)
      mZipPath = argZipPath
      fileToAdd = argfileToAdd
    End Sub

    Dim mZipPath As String = String.Empty
    Public Property ZipPath() As String
        Get
            Return mZipPath
        End Get
        Set(ByVal value As String)
            mZipPath = value
        End Set
    End Property

    Public Sub RenderData

Dim zip As Package = ZipPackage.Open(zipPath, _
          System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite)

Dim uriFileName As String = fileToAdd.Replace(" ", "_")

Dim zipUri As String = String.Concat("/", _
          System.IO.Path.GetFileName(uriFileName))

Dim partUri As New Uri(zipUri, UriKind.Relative)
	Dim contentType As String = _
          System.Net.Mime.MediaTypeNames.Application.Zip
Dim pkgPart As PackagePart = _
          zip.CreatePart(partUri, contentType, _
          CompressionOption.Normal)


Dim bites As Byte() = File.ReadAllBytes(fileToAdd)


pkgPart.GetStream().Write(bites, 0, bites.Length)

zip.Close() 'Close the zip file
    End Sub

End Class

"@ -ReferencedAssemblies 'System.IO.dll', 'System.dll','System.Net.dll','C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\WindowsBase.dll'
# can open the "WindowsBase.dll" assembly in c:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\ildasm.exe

# See Also: https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=netframework-4.5.1
# TODO: handle
# Exception calling "RenderData" with "0" argument(s): "Cannot add part for the specified URI because it is already in the package."
$caller = New-Object -TypeName 'Example' -ArgumentList ($archive, $filepath)
$caller.ZipPath = $archive
$caller.RenderData()

# Class InterleavedZipPartStream
# Class ZipArchive
# Class ZipFileInfo
# Class ZipPackagePart
# Class StreamingZipPartStream
# Class ZipPackage
# Class ZipFileInfoCollection
# Class ZipIOBlockManager
# Class ZipIOLocalFileBlock
# Class ZipIOCentralDirectoryBlock
# Class ZipIOZip64EndOfCentralDirectoryBlock
# Class ZipIOZip64EndOfCentralDirectoryLocatorBlock
# Class ZipIOEndOfCentralDirectoryBlock
# Interface IZipIOBlock
# Class ZipIORawDataFileBlock
# Class ZipIOVersionNeededToExtract
# Class ZipIOCentralDirectoryFileHeader
# Class ZipIOCentralDirectoryDigitalSignature
# Class ZipIOLocalFileHeader
# Class ZipIOExtraField
# Class ZipIOZip64ExtraFieldUsage
# Class ZipIOFileItemStream
# Class ZipIOLocalFileDataDescriptor
# Class ZipIOModeEnforcingStream
# Class ZipIOExtraFieldZip64Element
# Class ZipIOExtraFieldPaddingElement
# Class ZipIOExtraFieldElement

