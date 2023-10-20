# Copyright (c) 2023 Serguei Kouzmine
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the 'Software'), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.


param (
  [string]$files,
  [string]$regex,
  [switch]$numbers,
  [switch]$debug
)
add-type -typeDefinition @'
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Security;

public class Grep {

  private bool recursive;
  private bool ignoreCase;
  private bool justFilenames;
  private bool lineNumbers;
  private bool countLines;
  private string regEx;
  private string files;
  private ArrayList targets = new ArrayList();

  public bool Recursive {
    get { return recursive; }
    set { recursive = value; }
  }

  public bool IgnoreCase {
    get { return ignoreCase; }
    set { ignoreCase = value; }
  }

  public bool JustFilenames {
    get { return justFilenames; }
    set { justFilenames = value; }
  }

  public bool LineNumbers {
    get { return lineNumbers; }
    set { lineNumbers = value; }
  }

  public bool CountLines {
    get { return countLines; }
    set { countLines = value; }
  }

  public string RegEx {
    get { return regEx; }
    set { regEx = value; }
  }

  public string Files {
    get { return files; }
    set { files = value; }
  }


  private void GetFiles(String dir, String ext, bool recursive) {
    string[] f = Directory.GetFiles(dir, ext);
    for (int i = 0; i < f.Length; i++) {
      if (File.Exists(f[i]))
        targets.Add(f[i]);
    }
    if (recursive) {
      string[] d = Directory.GetDirectories(dir);
      for (int i = 0; i < d.Length; i++) {
        GetFiles(d[i], ext, true);
      }
    }
  }

  public void Search() {
    String dir = Environment.CurrentDirectory;
    
    targets.Clear();
    
    String[] f = files.Split(new Char[] { ',' });
    for (int i = 0; i < f.Length; i++) {
      f[i] = f[i].Trim();
      GetFiles(dir, f[i], recursive);
    }
    
    String results = "Grep results:\r\n\r\n";
    String line;
    int n, c;
    bool empty = true;
    IEnumerator t = targets.GetEnumerator();
    var regexOptions = (ignoreCase) ? RegexOptions.IgnoreCase |RegexOptions.Compiled : RegexOptions.Compiled;
          var regex = new Regex(regEx, regexOptions);
    while (t.MoveNext()) {
      try {
        StreamReader s = File.OpenText((string)t.Current);
        n = 0;
        c = 0;
        bool first = true;
        while ((line = s.ReadLine()) != null) {
          n++;
          // Match m = (ignoreCase) ? Regex.Match(line, regEx, RegexOptions.IgnoreCase) : Regex.Match(line, regEx);
          var m = regex.Match( line );					
          if (m.Success) {
            empty = false;
            c++;
            if (first) {
              if (justFilenames) {
                results += (string)t.Current + "\r\n";
                break;
              } else
                results += (string)t.Current + ":\r\n";
              first = false;
            }
            results += ((lineNumbers) ? "  " + n + ": " : "  " ) + line + "\r\n";
          }
        }
        s.Close();
        if (!first) {
          if (countLines)
            results += "  " + c + " Lines Matched\r\n";
          results += "\r\n";
        }
      } catch (SecurityException) {
        results += "\r\n" + (string)t.Current + ": Security Exception\r\n\r\n";	
      } catch (FileNotFoundException) {
        results += "\r\n" + (string)t.Current + ": File Not Found Exception\r\n";
      }
    }
    
    Console.WriteLine((empty) ? "No matches found!" : results);
    
  }

}

'@ -ReferencedAssemblies 'System.Data.dll', 'System.Text.RegularExpressions.dll'



$o = new-object Grep
$o.RegEx = $regex
$o.Files = $files
$o.Search()
