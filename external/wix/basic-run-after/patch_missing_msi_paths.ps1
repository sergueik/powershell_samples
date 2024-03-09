# based on: https://www.reddit.com/r/SCCM/comments/6larz4/missing_msi_filefailed_uninstall/

# put a purged msi into a specific network location and fix the registry information to support uninstall / upgrade
# MSI Error 1612: 'Installation source for this product is not available' during installation/uninstallation

$appname = "chrome"
    new-psdrive -name HKCR -psprovider Registry -root HKEY_CLASSES_ROOT
    # read All RegItems
    $items = get-childitem HKCR:Installer\Products -recurse -ErrorAction SilentlyContinue
    # get all WMI Product Objects
    $product = get-wmiobject win32_product
    # get the app GUID
    $products = $product | where-object {$_.name -like "*$($appname)*"} | Select IdentifyingNumber, Name
    foreach($product in $products){
        $identifyingNo = $product | select IdentifyingNumber
        
        # format GUID strings to search registry.
        $tidyString = $($identifyingNo.IdentifyingNumber).Replace("{","").Replace("}","".Replace(" ","")).Replace("-","")
        $charArray = $tidyString.ToCharArray()
        [string]$restructuredString = "$($charArray[7])$($charArray[6])$($charArray[5])$($charArray[4])$($charArray[3])$($charArray[2])$($charArray[1])$($charArray[0])$($charArray[11])$($charArray[10])$($charArray[9])$($charArray[8])$($charArray[15])$($charArray[14])$($charArray[13])$($charArray[12])$($charArray[17])$($charArray[16])$($charArray[19])$($charArray[18])$($charArray[21])$($charArray[20])$($charArray[23])$($charArray[22])$($charArray[25])$($charArray[24])$($charArray[27])$($charArray[26])$($charArray[29])$($charArray[28])$($charArray[31])$($charArray[30])"
        $key = ""
        # Find the 'SourceList\Net' keys for the installed apps
        foreach($item in $items){
            if($item.PSPath -like "*$($restructuredString)\SourceList\Net*"){
                $key = $($item.PSPath | Out-String).Replace("Microsoft.PowerShell.Core\Registry::","").Replace("HKEY_CLASSES_ROOT","HKCR").Replace("`n","")
                #Add the Reg Values for the app's network location
                $command = "REG ADD $($key) /v 2 /t REG_EXPAND_SZ /d `"\\servername\path\to\msi`" /f"
                cmd /c $command
            }
        }

        
    }
    # optionally restart SMS Service  - that will trigger App Deployment Eval Cycle on start to re-run the install
    get-service | where {$_.Name -eq "CCMExec"} | restart-service # restart the SMS Agent Host service

