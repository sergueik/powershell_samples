using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

// using EventLoggerHelper;

namespace RestoreDrive
{
    internal class RestoreDrive
    {
        #region Global Declarations

        // Win32 functions...
        [DllImport("Kernel32.dll", EntryPoint = "SetVolumeMountPointW", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetVolumeMountPoint(string VolumeMountPoint, string VolumeName);

        // Default values in case command line arguments are missing...
        private static string strRestoreDrive    = @"Z:\";   
        private static string strVolLabel        = "MyData";

        // Global messages...
        private static string strDriveLine       = "Drive Z: already exists, no need to continue...";
        private static string strFatalErrMsg     = "Fatal error restoring Z: drive, ";
        private static string strSuccessMsg      = "Restore was successful!  Z: is available...";

        // Event log variables... 
        private static string strAppName         = "RestoreDrive";
        private static string strLogName         = "myApps";
        private static long   lEventID           = 1001;

        // public  static EventLogger EventLogger { get; set; }
        #endregion

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// Main() is simple:                                                ///
        /// o Set up our custom event log                                    ///
        /// o Check to see if we need to restore the drive letter            ///
        /// o Execute the restore process, if necessary                      ///
        /// o Finish recording the event                                     ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        static void Main()
        {
            BeginLog();

            if (RestoreDriveInit())
            {
                RestoreDriveLetter();
            }                

            EndLog();

            return;
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// This method processes the command line arguments and then        ///
        /// checks to see if the drive letter is still assigned.             ///                       
        ///                                                                  ///
        /// There are three arguments:                                       ///
        /// o The app name (which is ignored)                                ///
        /// o The drive letter to be restored                                ///
        /// o The volume label the drive letter is assigned to               ///
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns>boolean</returns>
        private static bool RestoreDriveInit()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                // Arguments will be prefixed with a - , i.e. -Z
                //
                // Drive letter to be restored...
                if (args[1].Length > 0 && args[1][0].Equals('-'))
                {
                    strRestoreDrive = strRestoreDrive.Remove(0, 1).Insert(0, args[1][1].ToString()).ToUpper();
                }
                    
                // The volume label that will be used to find the GUID...
                if (args[2].Length > 0 && args[2][0].Equals('-'))
                {
                    strVolLabel = args[2].Substring(1);
                }                    
            }

            /*
            EventLogger.Add
            (
                "Parameters: Drive Letter = "    +
                strRestoreDrive.Substring(0, 2)  +
                ", Volume Label = "              +
                strVolLabel
            );
            */

            if (Directory.Exists(strRestoreDrive))
            {
                strDriveLine = strDriveLine.Remove(6, 1).Insert(6, strRestoreDrive.Substring(0, 1));
                // EventLogger.Add(strDriveLine);

                return false;
            }

            strSuccessMsg  = strSuccessMsg.Remove( 25, 1).Insert(25, strRestoreDrive.Substring(0, 1));
            strFatalErrMsg = strFatalErrMsg.Remove(22, 1).Insert(22, strRestoreDrive.Substring(0, 1));

            return true;
        }

        private static void RestoreDriveLetter()
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher
                (
                    "root\\CIMV2",
                    "SELECT DeviceID FROM Win32_Volume WHERE Label = '" + strVolLabel + "'"
                );                
                
                // If the Volume Label doesn't exist on the system, this statement throws a
                // NullReferenceException.  The following string.IsNullOrEmpty instruction is 
                // a "just in case" precaution; it shouldn't happen...
                string strVolGUID = mos.Get().Cast<ManagementObject>().FirstOrDefault().GetPropertyValue("DeviceID").ToString();

                if (string.IsNullOrEmpty(strVolGUID))
                {
                    // EventLogger.Add(strFatalErrMsg + "could not find the GUID for Volume Label = " + strVolLabel);
                }
                else
                { 
                    // The Device ID is the volume GUID.  Now that we have it we can restore the drive...
                    if (SetVolumeMountPoint(strRestoreDrive, strVolGUID))
                    {
                        // EventLogger.Add(strSuccessMsg);
                    }
                    else
                    {
                        // EventLogger.Add("Call to SetVolumeMountPoint() failed for Volume Label = " + strVolLabel);
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                if (mos!= null) 
                	mos.Dispose();
                // mos?.Dispose();
            }
            catch (Win32Exception ex)
            {   // For call to SetVolumeMountPoint()...
                // EventLogger.Add(strFatalErrMsg  + "error code = " + ex.NativeErrorCode.ToString() + ": " + ex.Message);
            }
            catch (ManagementException ex)
            {   // For Management query...
                // EventLogger.Add(strFatalErrMsg  + "error code = " + ex.ErrorCode.ToString() + ": " + ex.Message);
            }
            catch (NullReferenceException ex)
            {   // For Management object query when the Volume Label isn't found...
                // EventLogger.Add(strFatalErrMsg + "could not find the GUID for Volume Label = " + strVolLabel);
                // EventLogger.Add("Error code = " + ex.HResult.ToString() + ": " + ex.Message);
            }
            catch (Exception ex)    
            {   // Default...
                // EventLogger.Add(strFatalErrMsg  + "error code = " + ex.HResult.ToString()   + ": " + ex.Message);
            }

            return;
        }

        private static void BeginLog()
        {
            string strDate = DateTime.Now.ToString("MM/dd/yyyy",  CultureInfo.InvariantCulture);
            string strTime = DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

            // EventLogger    = new EventLogger(strAppName, strLogName, lEventID);

            // EventLogger.CreateEventLog();
            // EventLogger.Add(strAppName + " is starting on " + strDate + " at " + strTime + " ...");
            // EventLogger.Add("");
        }

        private static void EndLog()
        {
            string strDate = DateTime.Now.ToString("MM/dd/yyyy",  CultureInfo.InvariantCulture);
            string strTime = DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

           // EventLogger.Add("");
           // EventLogger.Add(strAppName + " is ending on " + strDate + " at " + strTime + " ...");
           // EventLogger.WriteToEventLog();

           // EventLogger.CloseEventLog();
           // EventLogger = null;
        }
    }
}
