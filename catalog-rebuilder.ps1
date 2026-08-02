#Copyright (c) 2026 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

param(
  [Parameter(Mandatory = $false,Position = 1)]
  [String]$datafile = 'catalog1.txt',
  [String]$target = 'test',
  [int]$count =  100
)


function initialize_data_reader {
  param(
    [string]$format = 'excel',
    [string]$datafile_filename,
    [string]$sheet_name,
    [string]$query,
    [System.Management.Automation.PSReference]$connection_ref,
    [System.Management.Automation.PSReference]$command_ref,
    [System.Management.Automation.PSReference]$data_table_ref,
    [bool]$debug

  )

  [string]$datafile_directory = (resolve-path -path '.').Path
  [string]$datafile_fullpath = ('{0}\{1}' -f $datafile_directory,$datafile_filename)

  switch ($format) {
    'excel' {
      [string]$oledb_provider = 'Provider=Microsoft.ACE.OLEDB.12.0'
      [string]$data_source = "Data Source = ${datafile_fullpath}"
      [string]$ext_arg = 'Extended Properties=Excel 8.0'
      [string]$table = $sheet_name
    }
    'excel_legacy' {
      # 32-bit instances only, Jet Engine has been included with core image for Windows XP, Server 2013
      [string]$oledb_provider = 'Provider=Microsoft.Jet.OLEDB.4.0'
      [string]$data_source = "Data Source = ${datafile_fullpath}"
      [string]$ext_arg = 'Extended Properties=Excel 8.0;IMEX=1;'
      [string]$table = $sheet_name
    }
    'csv' {
      [string]$oledb_provider = 'Provider=Microsoft.ACE.OLEDB.12.0'
      [string]$ext_arg = 'Extended Properties="Text;IMEX=1;HDR=Yes;FMT=Delimited(,)";'
      [string]$data_source = "Data Source = ${$datafile_directory}"
      [string]$table = $datafile_filename
    }
    'csv_legacy' {
      # 32-bit instances only:
      [string]$oledb_provider = 'Provider=Microsoft.Jet.OLEDB.4.0'
      [string]$ext_arg = 'Extended Properties="Text;IMEX=1;HDR=Yes;FMT=Delimited(,)";'
      [string]$data_source = "Data Source = ${$datafile_directory}"
      [string]$table = $datafile_filename
    }
    default { throw }
  }
  $connection_string = "$oledb_provider;$data_source;$ext_arg"
  
  [string]$query = "SELECT * FROM [${table}] WHERE ISNULL(guid)"

  [System.Data.OleDb.OleDbConnection]$local:connection = new-object System.Data.OleDb.OleDbConnection($connection_string)
  [System.Data.OleDb.OleDbCommand]$local:command = new-object System.Data.OleDb.OleDbCommand($query)

  [System.Data.DataTable]$local:data_table = new-object System.Data.DataTable
  [System.Data.OleDb.OleDbDataAdapter]$ole_db_adapter = new-object System.Data.OleDb.OleDbDataAdapter
  $ole_db_adapter.SelectCommand = $local:command

  $local:command.Connection = $connection

  [void]$ole_db_adapter.Fill($local:data_table)
  $local:connection.open()
  # http://stackoverflow.com/questions/24648081/error-the-type-system-data-oledb-oledbdatareader-has-no-constructors-defined
  $global:data_reader = $local:command.ExecuteReader()
  $data_table_ref.Value = $local:data_table
  $connection_ref.Value = $local:connection
  $command_ref.Value = $local:command
  return $local:data_reader
}

function insert_row_new {
  param(
    [string]$sql,
    [System.Data.OleDb.OleDbConnection]$connection,
    [System.Collections.Hashtable]$new_row_data
  )

  [string[]]$columns = [string[]]($row_data.Keys)

  [System.Data.OleDb.OleDbCommand]$local:command = new-object System.Data.OleDb.OleDbCommand
  $local:command.Connection = $connection

  $local:insert_name_part = @()
  $local:insert_value_part = @()

  $columns | ForEach-Object {
    $column_name = $_
    $column_data = $new_row_data[$column_name]
    $local:command.Parameters.Add(('@{0}' -f $column_name),$column_data['type']).Value = $column_data['value']
    Write-Output ('@{0} = {1}' -f $column_name,$column_data['value'])
    $local:insert_name_part += ('[{0}]' -f $column_name)
    $local:insert_value_part += ('@{0}' -f $column_name)
  }

  $local:generated_sql = (($sql -replace '@insert_name_part',($local:insert_name_part -join ',')) -replace '@insert_value_part',($local:insert_value_part -join ','))

  Write-Output ('Insert query: {0}' -f $local:generated_sql)

  $new_row_data.Keys | ForEach-Object {
    $column_name = $_
    $column_data = $new_row_data[$column_name]
    Write-Output ('@{0} = {1}' -f $column_name,$column_data['value'])
  }
  $local:command.CommandText = $local:generated_sql

  $local:result = $local:command.ExecuteNonQuery()

  Write-Output ('Insert result: {0}' -f $local:result)

  $local:command.Dispose()

  return $local:result

}



# NOTE: the original path remains the __source of truth__, while the derived columns are just search *aids*



[System.Collections.Hashtable]$t = @{
    # Languages;
    'java'        = 'java';
    'kotlin'      = 'kotlin';
    'scala'       = 'scala';
    'groovy'      = 'groovy';
    'python'      = 'python';
    'py'          = 'python';
    'javascript'  = 'javascript';
    'typescript'  = 'typescript';
    'node'        = 'node';
    'cobol'       = 'cobol';
    'fortran'     = 'fortran';

    # Java ecosystem;
    'spring'      = 'spring';
    'springboot'  = 'spring';
    'hibernate'   = 'hibernate';
    'maven'       = 'maven';
    'gradle'      = 'gradle';

    # Python ecosystem;
    'fastapi'     = 'fastapi';
    'django'      = 'django';
    'flask'       = 'flask';
    'pandas'      = 'pandas';

    # Web;
    'react'       = 'react';
    'angular'     = 'angular';
    'vue'         = 'vue';

    # Cloud / DevOps;
    'docker'      = 'docker';
    'aws'         = 'aws';
    'azure'       = 'azure';
    'kubernetes'  = 'kubernetes';
    'k8s'         = 'kubernetes';
    'terraform'   = 'terraform';

    # AI / Agent ecosystem;
    'mcp'         = 'mcp';
    'model-context-protocol' = 'mcp';
    'claude'      = 'claude';
    'openai'      = 'openai';
};

[String[]]$columns =  @( 'Skill Name', 'Category','Technology','Repository', 'Link');
[bool]$debug_flag  = $false
# git clone  --depth 1 https://github.com/majiayu000/claude-skill-registry
# write-host ('written {0}' -f $filepath)
$filepath = (resolve-path -path '.').path + '\' + $datafile

function proces_file { 

param(
)

$cnt = 0
$results = @();
write-host ('reading {0} rows from {1}' -f $count, $filepath)
$debug_flag = $false
get-content $filepath | foreach-object {
    $line = $_
    $cnt = $cnt + 1
    # [Void]$i.Items.Add($line)

    # claude-skill-registry/skills/agent/35-google-adk-reliable-agents/SKILL.md
    if ($debug_flag) {
      write-host ('read Data (raw):' + [char]10 + '"' + $line + '"' + [char]10)
    }
    $o = $null
    if ($cnt -gt $count ){
      # write-host ('count: {0}' -f $results.Count)
      # write-host ('example: {0}' -f ($results[0]|format-list))
      return ([ref]$results)
      # write-output ($results | format-list ) 
      # $results | foreach-object { write-output $_} 
      # WARNING - it is not what is appears
      # WARNING: old Powershell 2.0 parser understands it differently than 5.1  
      continue
      # https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_continue?view=powershell-5.1
      # continue :label
    }

    $pattern =  '^claude-skill-registry/skills/([^/]+)(?:/[^/]+)*/([^/]+)/SKILL.md$'
    # is not a valid regular expression: parsing
    # parsing "..." - Not enough )'s
    [Microsoft.PowerShell.Commands.MatchInfo]$m = $null
    $m = select-string -pattern $pattern -InputObject $line
    if (($m -ne $null ) -and ($m.matches -ne $null)) {
      try {
        $g = $m.Matches.Groups
        $c = $g.Item(1).Value
        $n = $g.Item(2).Value
        $a = @()
        $t.keys | foreach-object {
  	      $p = $_
  	      if (($n -match "${p}[^a-z]" ) -or  ($n -match "${p}$" )){
  		 $a +=$t[$p]
  	      }
        }
        write-host ('Category: {0} Skill Name: {1}'-f $c , $n )
        if ($a.count -ne 0 ) {
          write-host('Technology: {0}' -f ($a -join ',' ))
        }
  
        $r = @{ 'Skill Name' = $n;
                 'Category' = $c;
                 'Technology' = ( $a -join ',' );
                 'Link' = $line;
  	      };
        $results += $r 
        write-host ('Skill Name: {0}' -f $r['Skill Name'])
      } catch [InvalidOperationException] {
        # not outermost
      } catch [System.Management.Automation.RuntimeException] {
  	    write-host ("Exception (ignored): {0} {1}" -f $_.Exception.GetType().FullName, $_.Exception.Message)
        # https://devblogs.microsoft.com/scripting/troubleshoot-the-invokemethodonnull-error-with-powershell/
        # PowerShell FullyQualifiedErrorId : InvokeMethodOnNull (commonly stated as "You cannot call a method on a null-valued expression") means your code tries to run a method on a variable, object, or property that evaluates to $null
      }
      } 
    # Warning: retrofit
    # :label [void] 1
  }
}
$results_ref = proces_file
write-output ('==>{0} entries' -f $results_ref.value.Count)
$template = @'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1 plus MathML 2.0//EN" "http://www.w3.org/Math/DTD/mathml2/xhtml-math11-f.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en-US">
<!--This file was converted to xhtml by LibreOffice - see https://cgit.freedesktop.org/libreoffice/core/tree/filter/source/xslt for the code.-->

<head profile="http://dublincore.org/documents/dcmi-terms/">
<meta http-equiv="Content-Type" content="application/xhtml+xml; charset=utf-8"/>
<title xml:lang="en-US">- no title specified</title>
<meta name="DCTERMS.title" content="- no title specified" xml:lang="en-US"/>

<meta name="DCTERMS.language" content="en-US" scheme="DCTERMS.RFC4646"/>
<meta name="DCTERMS.source" content="http://xml.openoffice.org/odf2xhtml"/>



<meta name="DCTERMS.modified" content="2026-08-01T20:07:27.939000000" scheme="DCTERMS.W3CDTF"/>


<meta name="DCTERMS.issued" content="2026-08-01T08:12:30.085000000"/>
<meta name="DCTERMS.language" content="en-US"/>
<meta name="DCTERMS.modified" content="2026-08-01T19:49:54.998000000"/>
<meta name="DCTERMS.source" content="http://xml.openoffice.org/odf2xhtml"/>
<meta name="xsl:vendor" content="libxslt"/>
<link rel="schema.DC" href="http://purl.org/dc/elements/1.1/" hreflang="en"/>
<link rel="schema.DCTERMS" href="http://purl.org/dc/terms/" hreflang="en"/>
<link rel="schema.DCTYPE" href="http://purl.org/dc/dcmitype/" hreflang="en"/>
<link rel="schema.DCAM" href="http://purl.org/dc/dcam/" hreflang="en"/>

<style>
    table { border-collapse:collapse; border-spacing:0; empty-cells:show }
    td, th { vertical-align:top; font-size:10pt;}
    h1, h2, h3, h4, h5, h6 { clear:both;}
    p { white-space: nowrap; }
    ol, ul { margin:0; padding:0;}
    li { list-style: none; margin:0; padding:0;}
    span.footnodeNumber { padding-right:1em; }
    span.annotation_style_by_filter { font-size:95%; font-family:Arial; background-color:#fff000;  margin:0; border:0; padding:0;  }
    span.heading_numbering { margin-right: 0.8rem; }* { margin:0;}
    .table-ta1{ writing-mode:horizontal-tb; direction:ltr; }
    .cell-ce1{ font-size:12pt; font-family:Arial; font-style:normal; text-shadow:none; text-decoration:none ! important; font-weight:normal; }
    .cell-ce2{ font-size:12pt; font-family:Arial; }
    .col-co1{ width:0.4181in; }
    .col-co2{ width:2.4445in; }
    .col-co3{ width:1.3752in; }
    .col-co4{ width:2.3772in; }
    .col-co5{ width:3.7453in; }
    .col-co6{ width:0.889in; }
    .col-co7{ width:3.1618in; }
    .row-ro1{ height:0.2083in; }
    /* ODF styles with no properties representable as CSS:
     { } */
</style>
</head>

<body dir="ltr">


<table border="0" cellspacing="0" cellpadding="0" class="table-ta1"><colgroup><col width="46"/><col width="271"/><col width="153"/><col width="264"/><col width="416"/><col width="99"/><col width="99"/><col width="351"/><col width="99"/></colgroup><tr class="row-ro1"><td style="text-align:left;width:0.4181in; " class="cell-ce1">
<p>id</p>
</td><td style="text-align:left;width:2.4445in; " class="cell-ce1">
<p>Skill Name</p>
</td><td style="text-align:left;width:1.3752in; " class="cell-ce1">
<p>Category</p>
</td><td style="text-align:left;width:2.3772in; " class="cell-ce1">
<p>Technology</p>
</td><td style="text-align:left;width:3.7453in; " class="cell-ce1">
<p>Repository</p>
</td><td style="text-align:left;width:0.889in; " class="cell-ce1">
<p>Link</p>
</td><td style="text-align:left;width:0.889in; " class="cell-ce1">
<p>Select</p>
</td><td style="text-align:left;width:3.1618in; " class="cell-ce1">
<p>guid</p>
</td><td style="text-align:left;width:0.889in; " class="cell-ce1">
<p> </p>
</td></tr><tr class="row-ro1"><td style="text-align:right; width:0.4181in; " class="cell-ce1">
<p>ID</p>
</td><td style="text-align:left;width:2.4445in; " class="cell-ce1">
<p>SKILL NAME</p>
</td><td style="text-align:left;width:1.3752in; " class="cell-ce1">
<p>CATEGORY</p>
</td><td style="text-align:left;width:2.3772in; " class="cell-ce1">
<p>TECHNOLOGY</p>
</td><td style="text-align:left;width:3.7453in; " class="cell-ce1">
<p>REPOSITORY</p>
</td><td style="text-align:left;width:0.889in; " class="cell-ce1">
<p>LINK</p>
</td><td style="text-align:left;width:0.889in; " class="cell-ce1">
<p>SELECT</p>
</td><td style="text-align:left;width:3.1618in; " class="cell-ce1">
<p>GUID</p>
</td></tr>
</table>

</body>

</html>

'@
<#
$datafile_filename = 'catalog-template.xls'

$command = new-object System.Data.OleDb.OleDbCommand
$connection = new-object System.Data.OleDb.OleDbConnection

$sheet_name = 'Catalog$'
$data_table = new-object System.Data.DataTable

initialize_data_reader -datafile_filename $datafile_filename -sheet_name $sheet_name -connection_ref ([ref]$connection) -command_ref ([ref]$command) -data_table_ref ([ref]$data_table)
# https://learn.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbtype?view=netframework-4.5
# https://learn.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbparameter.oledbtype?view=netframework-4.5
$new_row_data = @{
  'id' = @{
    'value' = $row_num;
    'type' = [System.Data.OleDb.OleDbType]::Numeric;
  };
  'Skill Name' = @{
    'value' = '';
    'type' = [System.Data.OleDb.OleDbType]::VarChar;
  };
  'Category' = @{
    'value' = '';
    'type' = [System.Data.OleDb.OleDbType]::VarChar;
  };
  'Technology' = @{
    'value' = '';
    'type' = [System.Data.OleDb.OleDbType]::VarChar;
  };
  'Repository' = @{
    'value' = '';
    'type' = [System.Data.OleDb.OleDbType]::VarChar;
  };
  
  'Link' = @{
    'value' = '';
    'type' = [System.Data.OleDb.OleDbType]::Variant;
  };
  'Select' = @{
    'value' = $false;
    'type' = [System.Data.OleDb.OleDbType]::Boolean;
  };

  'guid' = @{
    'value' = ([guid]::NewGuid()).ToString();
    'type' = [System.Data.OleDb.OleDbType]::VarChar;
  };

}
# Exception calling "ExecuteNonQuery" with "0" argument(s): "Invalid bracketing of name '[]'."
insert_row_new `
   -connection $connection `
   -sql "Insert into [${sheet_name}] (@insert_name_part) values (@insert_value_part)" `
   -new_row_data $new_row_data

$command.Dispose()

$connection.close()
#>