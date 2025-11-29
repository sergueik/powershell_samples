using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using System.ComponentModel;
namespace ScreenMonitorLib
{

    class ImpersonateInteractiveUser : IDisposable
    {

        //[DllImport("advapi32", SetLastError = true),
        //SuppressUnmanagedCodeSecurityAttribute]
        //static extern int OpenProcessToken(
        //System.IntPtr ProcessHandle, // handle to process
        //uint DesiredAccess, // desired access to process
        //ref IntPtr TokenHandle // handle to open access token
        //);

        //[DllImport("kernel32", SetLastError = true),
        //SuppressUnmanagedCodeSecurityAttribute]
        //static extern bool CloseHandle(IntPtr handle);
        //[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
        //int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        //public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        //public const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
        //public const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;
        //public const UInt32 TOKEN_DUPLICATE = 0x0002;
        //public const UInt32 TOKEN_IMPERSONATE = 0x0004;
        //public const UInt32 TOKEN_QUERY = 0x0008;
        //public const UInt32 TOKEN_QUERY_SOURCE = 0x0010;
        //public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
        //public const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
        //public const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
        //public const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
        //public const UInt32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        //public const UInt32 TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
        //    TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
        //    TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
        //    TOKEN_ADJUST_SESSIONID);

        //public const int TOKEN_DUPLICATE = 2;
        //public const int TOKEN_QUERY = 0X00000008;
        //public const int TOKEN_IMPERSONATE = 0X00000004;

        WindowsImpersonationContext _impersonatedUser;
        IntPtr _hpiu = IntPtr.Zero;
        internal ImpersonateInteractiveUser(IntPtr hwnd)
        {

            IntPtr hToken = IntPtr.Zero;

            IntPtr dupeTokenHandle = IntPtr.Zero;
            uint procId;
            Win32API.GetWindowThreadProcessId(hwnd, out procId);
            Process proc = Process.GetProcessById((int)procId);

            Win32API.RevertToSelf();
            EventLog.WriteEntry("Screen Monitor", "user proc " + proc.ProcessName, EventLogEntryType.SuccessAudit, 1, 1);
            if (Win32API.OpenProcessToken(proc.Handle, TokenPrivilege.TOKEN_ALL_ACCESS,
            //TOKEN_QUERY | TOKEN_IMPERSONATE | TOKEN_DUPLICATE,
            ref hToken) != 0)
            {

                
                try
                {
                    const int SecurityImpersonation = 2;
                    dupeTokenHandle = DupeToken(hToken,
                    SecurityImpersonation);
                    if (IntPtr.Zero == dupeTokenHandle)
                    {
                        string s = String.Format("Dup failed {0}, privilege not held",
                        Marshal.GetLastWin32Error());
                        throw new Exception(s);
                    }
                    EventLog.WriteEntry("Screen Monitor", string.Format("Before impersonation: owner = {0}  Windows ID Name = {1} token = {2}", WindowsIdentity.GetCurrent().Owner, WindowsIdentity.GetCurrent().Name, WindowsIdentity.GetCurrent().Token), EventLogEntryType.SuccessAudit, 1, 1);
                    WindowsIdentity newId = new WindowsIdentity(dupeTokenHandle);

                    _impersonatedUser = newId.Impersonate();



                    WindowsIdentity WI = WindowsIdentity.GetCurrent();


                    Win32API.PROFILEINFO pi = new Win32API.PROFILEINFO();
                    pi.dwSize = Marshal.SizeOf(pi);
                    pi.lpUserName = WI.Name;
                    pi.dwFlags = 1;
                    // Allocate struct sized unmanaged memory
                    _hpiu = Marshal.AllocHGlobal(pi.dwSize);
                    Marshal.StructureToPtr(pi, _hpiu, true);

                    if (!Win32API.LoadUserProfile(WindowsIdentity.GetCurrent().Token, _hpiu))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                }
                finally
                {
                    Win32API.CloseHandle(hToken);
                    Win32API.CloseHandle(dupeTokenHandle);
                    EventLog.WriteEntry("Screen Monitor", string.Format("After impersonation: owner = {0}  Windows ID Name = {1} token = {2}", WindowsIdentity.GetCurrent().Owner, WindowsIdentity.GetCurrent().Name, WindowsIdentity.GetCurrent().Token), EventLogEntryType.SuccessAudit, 1, 1);
                }
            }
            else
            {
                string s = String.Format("OpenProcess Failed {0}, privilege not held", Marshal.GetLastWin32Error());
                throw new Exception(s);
            }

        }

        internal ImpersonateInteractiveUser(Process proc)
        {

            IntPtr hToken = IntPtr.Zero;

            IntPtr dupeTokenHandle = IntPtr.Zero;
            Win32API.RevertToSelf();
            EventLog.WriteEntry("Screen Monitor", "user proc " + proc.ProcessName, EventLogEntryType.SuccessAudit, 1, 1);
            if (Win32API.OpenProcessToken(proc.Handle,
            TokenPrivilege.TOKEN_ADJUST_PRIVILEGES | TokenPrivilege.TOKEN_QUERY
                 |TokenPrivilege.TOKEN_DUPLICATE|TokenPrivilege.TOKEN_ASSIGN_PRIMARY|TokenPrivilege.TOKEN_ADJUST_SESSIONID
                          |TokenPrivilege.TOKEN_READ,
            ref hToken) != 0)
            {


                try
                {
                    //const int SecurityImpersonation = 2;
                    //dupeTokenHandle = DupeToken(hToken,
                    //SecurityImpersonation);
                    //if (IntPtr.Zero == dupeTokenHandle)
                    //{
                    //    string s = String.Format("Dup failed {0}, privilege not held",
                    //    Marshal.GetLastWin32Error());
                    //    throw new Exception(s);
                    //}
                    EventLog.WriteEntry("Screen Monitor", string.Format("Before impersonation: owner = {0}  Windows ID Name = {1} token = {2}", WindowsIdentity.GetCurrent().Owner, WindowsIdentity.GetCurrent().Name, WindowsIdentity.GetCurrent().Token), EventLogEntryType.SuccessAudit, 1, 1);
                    WindowsIdentity newId = new WindowsIdentity(hToken);

                    _impersonatedUser = newId.Impersonate();



                    //WindowsIdentity WI = WindowsIdentity.GetCurrent();


                    //Win32API.PROFILEINFO pi = new Win32API.PROFILEINFO();
                    //pi.dwSize = Marshal.SizeOf(pi);
                    //pi.lpUserName = WI.Name;
                    //pi.dwFlags = 1;
                    //// Allocate struct sized unmanaged memory
                    //_hpiu = Marshal.AllocHGlobal(pi.dwSize);
                    //Marshal.StructureToPtr(pi, _hpiu, true);

                    //if (!Win32API.LoadUserProfile(WindowsIdentity.GetCurrent().Token, _hpiu))
                    //{
                    //    throw new Win32Exception(Marshal.GetLastWin32Error());
                    //}

                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Screen Monitor", string.Format("impersonation failed: msg = {0}", WindowsIdentity.GetCurrent().Owner, ex.Message), EventLogEntryType.SuccessAudit, 1, 1);
                }
                finally
                {
                    if(hToken != IntPtr.Zero)
                        Win32API.CloseHandle(hToken);
                    if (dupeTokenHandle != IntPtr.Zero)
                        Win32API.CloseHandle(dupeTokenHandle);
                    EventLog.WriteEntry("Screen Monitor", string.Format("After impersonation: owner = {0}  Windows ID Name = {1} token = {2}", WindowsIdentity.GetCurrent().Owner, WindowsIdentity.GetCurrent().Name, WindowsIdentity.GetCurrent().Token), EventLogEntryType.SuccessAudit, 1, 1);
                }
            }
            else
            {
                string s = String.Format("OpenProcess Failed {0}, privilege not held", Marshal.GetLastWin32Error());
                throw new Exception(s);
            }

        }

        static IntPtr DupeToken(IntPtr token, int Level)
        {
            IntPtr dupeTokenHandle = IntPtr.Zero;
            bool retVal = Win32API.DuplicateToken(token, Level, ref dupeTokenHandle);
            return dupeTokenHandle;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //if (!Win32API.UnloadUserProfile(WindowsIdentity.GetCurrent().Token, _hpiu))
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //Marshal.FreeHGlobal(_hpiu);
            if(_impersonatedUser != null)
            {
            _impersonatedUser.Undo();
            _impersonatedUser.Dispose();
            }
            EventLog.WriteEntry("Screen Monitor", string.Format("After undo impersonation: owner = {0}  Windows ID Name = {1} token = {2}", WindowsIdentity.GetCurrent().Owner, WindowsIdentity.GetCurrent().Name, WindowsIdentity.GetCurrent().Token), EventLogEntryType.SuccessAudit, 1, 1);
        }

        #endregion
    }
}

