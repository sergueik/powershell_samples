# origin: https://www.cyberforum.ru/powershell/thread3166458.html#post17298555
$RunningWallpaper = "${env:USERPROFILE}\Downloads\1.jpg"
$NotRunningWallpaper = "${env:USERPROFILE}\Downloads\2.png"
 

function Set-Wallpaper {
  param (
    [string]$ImagePath
  )
  $code = @'
  using System;
  using System.Runtime.InteropServices;
 
  public class Wallpaper {
    // http://www.pinvoke.net/default.aspx/user32.systemparametersinfo
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    
    public const uint SPI_SETDESKWALLPAPER = 0x14;
    public const int SPIF_UPDATEINIFILE = 0x01;
    public const int SPIF_SENDCHANGE = 0x02;
 
    public static void SetDesktopWallpaper(string path) {
      SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }
  }
'@
  Add-Type -TypeDefinition $code -Language CSharp
  [Wallpaper]::SetDesktopWallpaper($ImagePath)
}
 

$Process = Get-Process "chrome"
 
if ($Process) {
 
  Set-Wallpaper -ImagePath $RunningWallpaper
} else {
 
  Set-Wallpaper -ImagePath $NotRunningWallpaper
}
