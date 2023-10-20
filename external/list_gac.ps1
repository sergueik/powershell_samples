# origin: http://www.cyberforum.ru/blogs/579090/blog3958.html

<#

using System;
using System.Reflection;
using System.Collections;
 
internal sealed class Program {
  const UInt32 ASM_CACHE_GAC = 0x2;
  
  static void Main(String[] args) {
    if (args.Length > 1) {
      Console.WriteLine("Index is out of range.");
      return;
    }
    
    ArrayList al = new ArrayList();
    String assem = args.Length == 1 ? args[0] : null;
    
    MethodInfo ReadCache = Assembly
        .GetAssembly(typeof(Object))
        .GetType("Microsoft.Win32.Fusion")
        .GetMethod("ReadCache");
    ReadCache.Invoke(null, new Object[] {al, assem, ASM_CACHE_GAC});
    foreach (String a in al) Console.WriteLine(a);
  }
}

#>