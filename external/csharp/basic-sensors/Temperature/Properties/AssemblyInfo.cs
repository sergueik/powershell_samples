using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Temperature")]
[assembly: AssemblyDescription("Displays CPU Core temperature")]
[assembly: AssemblyConfiguration("1.0.4.0")]
[assembly: AssemblyCompany("HC Williams")]
[assembly: AssemblyProduct("Temperature")]
[assembly: AssemblyCopyright("Copyright © 2019 GNU V3 public license")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c51eaab4-f0e1-4046-8b58-114421faa377")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
// revisions:
// 11-15-2014:  Changed appearance and window location, added contextmenu
// 12-09-2014:  changed to single instance of OpenHardware in each form to avoid memory overflow crash
// 01-21-2015:  Converted Hardware Display to listview; set Topmost = true;
// 01-24-2015:  Changed update interval to 60sec, added update menu item
// 01-26-2015:  Added Timer Interval Menu, MaxTemp
// 02-13-2016:  Added AppManifest with <requestedExecutionLevel  level="highestAvailable" uiAccess="false" />
//              This is required for proper operation in current Windows 7 and 10
//              Go to Project, add item, application manifest and edit the xml code displayed.
// 02-20-2016:  Fixed embedded png stream loader for AboutBox
// 07-28-2016:  Fixed SubHardware.Update() issue so more SH items show up in list
// 07-28-2016:  Now using single common wrapper class to access OHM DLL
//              Added WIN_32 BIOS Information Class
// 07-29-2016:  Added Clipboard Copy
// 01-09-2017:  Added Update ability and OS information to Form2
// 01-21-2017:  Rebuilt with OHM dll ver 0.8.0.0 beta
// 01-21-2017:  Added memory usage information
// 08-02-2017:  Added Async Temp updating using BackgroundWorker
// 09-14-2017:  Added Display Resolution
// 10-24-2017:  Added Detailed Windows Version & Release Info
// 10-27-2017:  Added code to exclude blank Data and Level Sensors currently missing data from OHMonitor
// 10-30-2017:  Changed Temp in main window to Average of Available CPU temps
// 01-05-2019:  Added Windows Build number
// 11-05-2019:  Adding settings feature
// 11-09-2019:  Completed settings with Opacity Preview
// 11-12-2019:  Completed Save and Load Settings file
// 11-17-2019:  Adding Charting feature
// 11-24-2019:  Finished Charting, added log start time 

[assembly: AssemblyVersion("1.0.4.0")]
[assembly: AssemblyFileVersion("1.0.4.0")]
