using Microsoft.Win32;
using System;
using System.IO;
using System.Security;
using System.Windows.Forms;

static class StartupRegistrar
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "WSLManager";

    // .NET Framework ではこれでOK
    private static string ExePath
    {
        get { return Application.ExecutablePath; }
    }

    public static bool IsEnabled()
    {
        using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
        {
            var value = key == null ? null : key.GetValue(AppName) as string;
            if (string.IsNullOrEmpty(value)) return false;

            var expectedQuoted = "\"" + ExePath + "\"";
            return value.StartsWith(expectedQuoted, StringComparison.OrdinalIgnoreCase)
                || value.StartsWith(ExePath, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static void SetEnabled(bool enable, string startupArgs = null)
    {
        try
        {
            // .NET Framework では「OpenSubKey(..., true)」→ なければ「CreateSubKey(...)」
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(RunKeyPath);

            if (key == null)
                throw new InvalidOperationException("Run キーを作成/開けませんでした。");

            using (key)
            {
                if (enable)
                {
                    var command = "\"" + ExePath + "\"";
                    if (!string.IsNullOrEmpty(startupArgs))
                        command += " " + startupArgs;

                    key.SetValue(AppName, command, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            ShowError("権限が不足しているため、スタートアップ設定を変更できませんでした。管理者または組織ポリシーをご確認ください。");
            throw;
        }
        catch (SecurityException)
        {
            ShowError("セキュリティポリシーにより操作がブロックされました。");
            throw;
        }
        catch (IOException)
        {
            ShowError("レジストリへのアクセス中に入出力エラーが発生しました。");
            throw;
        }
        catch (Exception ex)
        {
            ShowError("スタートアップ設定に失敗しました: " + ex.Message);
            throw;
        }
    }

    private static void ShowError(string message)
    {
        try
        {
            MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch { /* サービス化などで UI が無い場合に備えて無視 */ }
    }
}
