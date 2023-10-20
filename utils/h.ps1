Add-Type -TypeDefinition @'
using System;
public class Helper
{
public static void Message(String message) {
Console.Error.WriteLine(message);
}
}
'@
new-object -typeName Helper