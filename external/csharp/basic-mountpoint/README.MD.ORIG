# Restore Drive Letter

I like to keep my data on a separate logical drive I’ve assigned the drive letter Z.  Been that way for a long time.  It makes it easier to back up, search, etc.  Then starting with Windows 10, I began experiencing a problem where my Z: drive would just disappear!  Somehow the drive letter got dropped.  At first, I panicked—I thought I lost everything!  Then I realized it was just the drive letter itself; the partition was intact.  So, I used Disk Management to reassign the drive letter.  Problem is, it kept happening at random and for no apparent reason.  For a while, I just kept the Disk Management tool open on my desktop.  It became tedious very quickly.

I’m a retired software engineer so I decided to create my own fix for the problem.  I began scouring the internet to see if there was an automated solution out there.  I didn't find anything that was really useful so I created a batch file using Diskpart that got the job done.  Once I got it working, I created a scheduled task to run at logon and every hour thereafter.

I hadn’t programmed in a while so it was fun.  So much fun I decided to write it in a few other languages just for the exercise: **C#, C++/CLI, Visual Basic, Java, Python** and, believe it or not, **COBOL**.  In any case, the C# version is the one I wish to share here.  It’s a very simple program that utilizes the Windows API function *SetVolumeMountPoint* to assign the drive letter and records the results in a custom event log.  It takes two command line arguments: the drive letter and the volume label, each preceded by a dash (-), i.e.: *-X -YourVolumeLabel*.  It then uses System.Management to query WMI using the volume label to get its GUID which, in turn, gets passed to SetVolumeMountPoint to reassign the drive letter.  I could have used Diskpart or Powershell or even mountvol to get the GUID and then passed that as a command line argument but I chose to use the volume label because it’s readily visible in the system (i.e., in File Explorer and Disk Management), no look-up is necessary.  Here's the relevent code snippet:

```
// Use Linq to query WMI using System.Management...
ManagementObjectSearcher mos = new ManagementObjectSearcher
(
    "root\\CIMV2",
    "SELECT DeviceID FROM Win32_Volume WHERE Label = '" + strVolLabel + "'"
);                
        
// Get the Volume GUID
string strVolGUID = mos.Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("DeviceID").ToString();

if (string.IsNullOrEmpty(strVolGUID))
{
    Write a failure message to the event log...
}
else
{ 
    // The Device ID is the volume GUID.  Now that we have it we can restore the drive...
    if (SetVolumeMountPoint(strRestoreDrive, strVolGUID))
    {
        Write a successful message to the event log...
    }
    else
    {
        Write a failure message to the event log...
        throw a Win32Exception
    }
}
```                    

It's only a couple of pages of code: one for the restore process itself and the other for a helper class I created to handle the custom event log.  It's pretty thoroughly commented.  That’s pretty much it.  I hope you find it useful!