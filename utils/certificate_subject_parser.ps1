# https://stackoverflow.com/questions/7688445/extract-common-name-from-distinguished-name

string dn = "CN=TestGroup,OU=Groups,OU=UT-SLC,OU=US,DC=Company,DC=com";
Assembly dirsvc = Assembly.Load("System.DirectoryServices");
Type asmType = dirsvc.GetType("System.DirectoryServices.ActiveDirectory.Utils");
MethodInfo mi = asmType.GetMethod("GetDNComponents", BindingFlags.NonPublic | BindingFlags.Static);
string[] parameters = { dn };
var test = mi.Invoke(null, parameters);
//test.Dump("test1");//shows details when using Linqpad 

//Convert Distinguished Name (DN) to Relative Distinguished Names (RDN) 
MethodInfo mi2 = asmType.GetMethod("GetRdnFromDN", BindingFlags.NonPublic | BindingFlags.Static);
var test2 = mi2.Invoke(null, parameters);
//test2.Dump("test2");//shows details when using Linqpad 

# see also:
# https://www.cyberforum.ru/powershell/thread2856173.html
# (cryptic)

#requires -version 7
$GetDNComponents = [DirectoryServices.SortOption].Assembly.GetType(
  'System.DirectoryServices.ActiveDirectory.Utils'
).GetMethod(
  'GetDNComponents', [Reflection.BindingFlags]'NonPublic, Static'
)
 
(Get-ChildItem cert:\CurrentUser\My).Subject.ForEach{
  $GetDNComponents.Invoke($null, @($_)).Where{
    $_.Name -in 'CN', 'E', 'SN'
  }.ForEach{
    Set-Variable -Name $_.Name -Value $_.Value
  }
  # что-нибудь делается с переменными $cn, $e и $sn
  # срок существования переменных ограничивается итерацией
}

