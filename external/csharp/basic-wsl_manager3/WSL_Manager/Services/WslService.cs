using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WslManagerFramework.Models;

namespace WslManagerFramework.Services
{
    public class WslService
    {
        private static string GetWslPath()
        {
            var systemDir = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                ? @"C:\Windows\Sysnative"
                : @"C:\Windows\System32";
            return System.IO.Path.Combine(systemDir, "wsl.exe");
        }

        public static string[] ListDistros()
        {
            var psi = new ProcessStartInfo
            {
                FileName = GetWslPath(),
                Arguments = "-l -q",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Unicode
            };

            using (var p = new Process { StartInfo = psi })
            {
                if (!p.Start())
                    throw new InvalidOperationException("wsl.exe の起動に失敗しました。");

                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode != 0)
                    throw new InvalidOperationException("wsl.exe エラー: " + error);

                var cleanOutput = output.Replace("\0", "");
                var lines = cleanOutput
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.StartsWith("*") ? s.Substring(1).Trim() : s)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToArray();

                return lines;
            }
        }

        public static string GetDistroStatus(string distroName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = GetWslPath(),
                    Arguments = "-l -v",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode
                };

                using (var p = new Process { StartInfo = psi })
                {
                    if (!p.Start())
                        return "Unknown (Failed to start)";

                    string output = p.StandardOutput.ReadToEnd();
                    string error = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    	return String.Format("Unknown (Exit code: {0})",p.ExitCode);

                    var cleanOutput = output.Replace("\0", "");
                    var lines = cleanOutput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.Contains("NAME") || trimmedLine.Contains("----"))
                            continue;

                        var cleanLine = trimmedLine;
                        if (cleanLine.StartsWith("*"))
                        {
                            cleanLine = cleanLine.Substring(1).Trim();
                        }
                        
                        var parts = cleanLine.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            var name = parts[0];
                            var status = parts[1];
                            
                            if (name.Equals(distroName, StringComparison.OrdinalIgnoreCase))
                            {
                                return status;
                            }
                        }
                    }

                    return "Unknown (Not found)";
                }
            }
            catch (Exception ex)
            {
            	return String.Format("Unknown (Exception: {0})",ex.Message);
            }
        }

        public static async Task LaunchWslBackgroundAsync(string distroName)
        {
            var cleanName = distroName.Trim();
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("-d {0} --exec echo 'WSL started'",cleanName),
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
                    await Task.Run(() => process.WaitForExit(10000));
                    
                    if (process.ExitCode != 0)
                    {
                        var error = await Task.Run(() => process.StandardError.ReadToEnd());
                        throw new Exception(String.Format("{0} の起動に失敗しました。エラー: {1}",cleanName,error));
                    }
                }
            }
        }

        public static async Task StopWslAsync(string distroName)
        {
            var cleanName = distroName.Trim();
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("--terminate {0}",cleanName),
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
                    await Task.Run(() => process.WaitForExit(10000));
                    
                    if (process.ExitCode != 0)
                    {
                        string error = await Task.Run(() => process.StandardError.ReadToEnd());
                        throw new Exception(String.Format("{0} の停止に失敗しました。エラー: {1}",cleanName, error));
                    }
                }
            }
        }

        public static void LaunchInCmd(string distroName)
        {
            var cleanName = distroName.Trim();
            var wslPath = GetWslPath();
            var commandLine = String.Format("/k \"\"{0}\" -d {0}\"",wslPath, cleanName);
            
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = commandLine,
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };
            Process.Start(psi);
        }

        public static void LaunchWslDirect(string distroName)
        {
            var cleanName = distroName.Trim();
            var wslPath = GetWslPath();

            var psi = new ProcessStartInfo
            {
                FileName = wslPath,
                Arguments = String.Format("-d {0}" , cleanName),
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };
            Process.Start(psi);
        }
    }
}