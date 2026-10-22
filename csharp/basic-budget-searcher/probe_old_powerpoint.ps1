
param (
  [string] $filename = 'file_example_PPT_500kB.ppt',
  [switch] $debug # currently unused
)

$filename = Join-Path (resolve-path -path '.').Path $filename
[bool]$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'

# NOTE: with Powershell ISE the '.' will be  'c:\WINDOWS\system32'

pushd 'C:\developer\sergueik\powershell_samples\csharp\basic-budget-searcher'
$shared_assemblies_path = (resolve-path -path '.').Path

write-host ('Loading shared assembliesn from {0}' -f $shared_assemblies_path )

$shared_assemblies  = @(
  'ICSharpCode.SharpZipLib.dll',
  'NPOI.OOXML.dll',
  'NPOI.OpenXml4Net.dll',
  'NPOI.dll',
  'UglyToad.PdfPig.DocumentLayoutAnalysis.dll',
  'UglyToad.PdfPig.dll'
)


pushd $shared_assemblies_path
$shared_assemblies_full_paths = @()

$shared_assemblies | foreach-object {
  if ($host.Version.Major -gt 2) {
    Unblock-File -Path $_
  }
  $shared_assembly = (resolve-path -path $_).Path
  if ($debug_flag) {
    write-host ('loading {0}' -f $shared_assembly)
  }
  $shared_assemblies_full_paths += $shared_assembly 
  try {
    [Reflection.Assembly]::LoadFrom($shared_assembly)
  } catch [Exception] {
    write-host ('{0}' -f $_.Exception.LoaderExceptions )
 }
}
popd
if ($debug_flag) {
  [AppDomain]::CurrentDomain.GetAssemblies() |
  where-object { $_.GetName().Name -like '*PdfPig*' } |
    forEach-object {
      write-host ('Fullname: {0} Location: {1}' -f $_.FullName, $_.Location )
  }
}    




# TODO : Compile the small C# probe only once per PowerShell session.
# if (-not ('LegacyPptProbe.Harness' -as [type])) {

    $source = @'
using System;
using System.Collections.Generic;
using System.IO;
using NPOI;
using NPOI.OpenXml4Net.OPC;
using NPOI.Util;

namespace LegacyPptProbe
{
    public sealed class ProbeDocument : POIXMLDocument
    {
        public ProbeDocument(OPCPackage package)
            : base(package)
        {
        }

        public void Parse(POIXMLFactory factory)
        {
            Load(factory);
        }

        public override List<PackagePart> GetAllEmbedds()
        {
            throw new NotSupportedException(
                "Embedded-part enumeration is not needed by this probe.");
        }
    }

    public sealed class TestFactory : POIXMLFactory
    {
        protected override POIXMLRelation GetDescriptor(
            string relationshipType)
        {
            // Treat relationships as unknown, just as the minimal
            // Apache POI test factory does.
            return null;
        }

        protected override POIXMLDocumentPart CreateDocumentPart(
            Type cls, Type[] classes, object[] values)
        {
            // GetDescriptor returns null, so the base factory should
            // use its default POIXMLDocumentPart path instead.
            throw new NotSupportedException(
                "Unexpected call to the custom document-part constructor.");
        }
    }

    public static class Harness
    {
        public static string LastStage { get; private set; }

        public static Exception CaptureFailure(string filename)
        {
            LastStage = "File.OpenRead";

            try
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    LastStage = "PackageHelper.Open";

                    OPCPackage package = PackageHelper.Open(stream);

                    ProbeDocument doc = new ProbeDocument(package);

                    try
                    {
                        LastStage = "doc.Parse(TestFactory)";
                        doc.Parse(new TestFactory());

                        // No exception occurred.
                        return null;
                    }
                    finally
                    {
                        doc.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Return the original CLR exception, avoiding any
                // PowerShell method-invocation wrapper in the assertion.
                return ex;
            }
        }
    }
}
'@

$shared_assemblies_full_paths += 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll', 'System.Collections.dll', 'System.xml.dll','System.Xml.Linq.dll'
add-type -typedefinition $source -language CSharp -ReferencedAssemblies $shared_assemblies_full_paths

$failure = [LegacyPptProbe.Harness]::CaptureFailure( $filename)
write-output $failure 
write-output $failure.GetType().FullName
write-output $failure.Message
write-output ([LegacyPptProbe.Harness]::LastStage)
<# Wrong Local header signature: 0xE011CFD0 

ICSharpCode.SharpZipLib.Zip.ZipException
Wrong Local header signature: 0xE011CFD0
PackageHelper.Open
#>
<#
The hexadecimal value is particularly revealing. 
A legacy .ppt file starts with the OLE Compound File signature:
od -h file_example_PPT_500kB.ppt  |head -1
0000000 cfd0 e011 b1a1 e11a 0000 0000 0000 0000
Read as a little-endian 32-bit integer, those bytes become 0xE011CFD0
#>
<#

Describe 'NPOI low-level parsing of a legacy PowerPoint file' {

    It 'reports the expected SharpZipLib exception' {

        $failure = [LegacyPptProbe.Harness]::CaptureFailure(
            $filename)

        ($null -ne $failure) | Should Be $true

        if ($null -ne $failure) {

            $failure.GetType().FullName |
                Should Be 'ICSharpCode.SharpZipLib.Zip.ZipException'

            $failure.Message.StartsWith(
                'Wrong Local header signature',
                [System.StringComparison]::Ordinal
            ) | Should Be $true

            [LegacyPptProbe.Harness]::LastStage |
                Should Be 'PackageHelper.Open'
        }
    }
}

#>
