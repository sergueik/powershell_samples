#Copyright (c) 2023 Serguei Kouzmine
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


# https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string

param(
  [String]$contents = '',
  [string]$filename = 'a.txt',
  [switch]$decode
)



add-type -TypeDefinition @"
using System;
using System.Text;

public class Convertor {
    private string _filename;
    private string _encodedData;

    public string Filename
    {
        get { return _filename; }
        set { _filename = value; }
    }
    public string EncodedData
    {
        get { return _encodedData; }
        set { _encodedData = value; }
    }
    public void convert() {

            string data = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(_encodedData));

            if (!string.IsNullOrEmpty(_filename))
            {
              System.IO.File.WriteAllText(_filename, data);
              return ;
            }

    }
}

"@ -ReferencedAssemblies 'System.dll'


$o = new-object Convertor
$o.Filename = $filename
$o.EncodedData = $contents
$o.convert()
# TODO: read data from the file and encode
<#
[String]$encoded = [System.Convert]::ToBase64String([System.Text.Encoding]::GetEncoding('ASCII').GetBytes($raw))
[String]$raw = [System.Text.Encoding]::GetEncoding('UTF8').GetString([System.Convert]::FromBase64String($encoded))
#>
# see also: https://java2blog.com/write-binary-files-powershell/
[IO.File]::WriteAllBytes('.\test2.txt', ([System.Convert]::FromBase64String($contents)))

[Byte[]]$data = [System.Convert]::FromBase64String($contents)
add-content -LiteralPath '.\test3.txt' -Value $data -Encoding Byte
add-content -LiteralPath '.\test4.txt' -Encoding Byte -Value ([System.Convert]::FromBase64String($contents))
# the opposite is get-content -encoding Byte -literalpath $filename
# or get-cotent -rawe -literalpath $filename
<#
Testing:
* in git bash, hash a string
echo 'test 123' | base64 -
dGVzdCAxMjMK
* with this powershell script decode it and  compare
./convert.ps1 -contents 'dGVzdCAxMjMK' -filename '1.txt'
* in git bash, hash the file 
base64 '1.txt'
dGVzdCAxMjMK
* in git bash, hash a string extracted from the file
base64.exe '1.txt'./convert.ps1 -contents 'dGVzdCAxMjMK' -filename '1.txt'
cat '1.txt'  | base64 -
dGVzdCAxMjMK
#>
