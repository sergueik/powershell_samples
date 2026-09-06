using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WslManagerFramework.Models;

namespace WslManagerFramework.Services
{
    public class WslInstallService
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        
        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);
        
        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(IntPtr hThread);
        
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private static string GetWslPath()
        {
            var systemDir = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                ? @"C:\Windows\Sysnative"
                : @"C:\Windows\System32";
            return System.IO.Path.Combine(systemDir, "wsl.exe");
        }

        public static List<AvailableDistro> GetAvailableDistros()
        {
            var availableDistros = new List<AvailableDistro>();
            
            try
            {
                var wslPath = GetWslPath();

                var psi = new ProcessStartInfo
                {
                    FileName = wslPath,
                    Arguments = "--list --online",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode
                };

                using (var process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            var cleanOutput = output.Replace("\0", "");
                            var lines = cleanOutput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            bool dataStarted = false;
                            for (int i = 0; i < lines.Length; i++)
                            {
                                var line = lines[i];
                                var trimmedLine = line.Trim();
                                
                                if (trimmedLine.Contains("インストールできる有効なディストリビューション") ||
                                    trimmedLine.Contains("wsl.exe --install") ||
                                    trimmedLine.Contains("NAME") ||
                                    trimmedLine.Contains("----") ||
                                    string.IsNullOrWhiteSpace(trimmedLine))
                                {
                                    if (trimmedLine.Contains("NAME"))
                                    {
                                        dataStarted = true;
                                    }
                                    continue;
                                }

                                if (dataStarted)
                                {
                                    var parts = trimmedLine.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                                    
                                    if (parts.Length >= 1)
                                    {
                                        var name = parts[0];
                                        var friendlyName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : name;
                                        
                                        availableDistros.Add(new AvailableDistro(name, friendlyName, String.Format("{0} ディストリビューション",friendlyName)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // エラーの場合は空のリストを返す
            }

            return availableDistros;
        }

        public static void InstallDistro(string distroName)
        {
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("--install -d {0}",distroName),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };

            using (var process = Process.Start(psi))
            {
                if (process != null)
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new Exception(String.Format("インストールプロセスが失敗しました (終了コード: {0}): {1}",process.ExitCode,error));
                    }
                }
            }
        }

        public static void InstallDistroWithProgress(string distroName, InstallProgress progress, string installName)
        {
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("--install -d {0}",distroName),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };

            using (var process = Process.Start(psi))
            {
                if (process != null)
                {
                    progress.InstallProcess = process;
                    
                    while (!process.HasExited)
                    {
                        if (progress.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            process.Kill();
                            throw new OperationCanceledException();
                        }
                        
                        System.Threading.Thread.Sleep(100);
                    }
                    
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new Exception(String.Format("インストールプロセスが失敗しました (終了コード: {0}): {1}",process.ExitCode,error));
                    }
                }
            }
        }

        public static void InstallDistroWithCustomNameAndProgress(string distroName, string customName, InstallProgress progress)
        {
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("--install -d {0} --name {1}",distroName,customName),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };

            using (var process = Process.Start(psi))
            {
                if (process != null)
                {
                    progress.InstallProcess = process;
                    
                    while (!process.HasExited)
                    {
                        if (progress.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            process.Kill();
                            throw new OperationCanceledException();
                        }
                        
                        System.Threading.Thread.Sleep(100);
                    }
                    
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new Exception(String.Format("カスタム名インストールプロセスが失敗しました (終了コード: {0}): {1}",process.ExitCode,error));
                    }
                }
            }
        }

        public static void SuspendProcess(Process process)
        {
            if (process == null || process.HasExited) return;
            
            try
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    var threadHandle = OpenThread(2, false, (uint)thread.Id);
                    if (threadHandle != IntPtr.Zero)
                    {
                        SuspendThread(threadHandle);
                        CloseHandle(threadHandle);
                    }
                }
            }
            catch (Exception)
            {
                // エラー処理は呼び出し元で実行
            }
        }

        public static void ResumeProcess(Process process)
        {
            if (process == null || process.HasExited) return;
            
            try
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    var threadHandle = OpenThread(2, false, (uint)thread.Id);
                    if (threadHandle != IntPtr.Zero)
                    {
                        ResumeThread(threadHandle);
                        CloseHandle(threadHandle);
                    }
                }
            }
            catch (Exception)
            {
                // エラー処理は呼び出し元で実行
            }
        }
    }
}