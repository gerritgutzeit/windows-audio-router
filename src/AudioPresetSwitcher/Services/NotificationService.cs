using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using AudioPresetSwitcher.Models;
using Microsoft.Toolkit.Uwp.Notifications;

namespace AudioPresetSwitcher.Services;

public sealed class NotificationService
{
    public const string AppUserModelId = "Gerrit.AudioPresetSwitcher";

    private readonly SettingsService _settings;

    public NotificationService(SettingsService settings)
    {
        _settings = settings;
    }

    public void Initialize()
    {
        try
        {
            NativeMethods.SetCurrentProcessExplicitAppUserModelID(AppUserModelId);
            ShortcutHelper.Ensure(AppUserModelId, "AudioPresetSwitcher");
        }
        catch
        {
            // Toasts may still work; tray balloon is the fallback.
        }
    }

    public void ShowPresetResult(PresetActivationResult result)
    {
        if (!_settings.Current.ShowToastNotifications)
        {
            return;
        }

        Show(result.AllRequestedSucceeded ? "Audio preset switched" : "Audio preset", result.Summary);
    }

    public void Show(string title, string message)
    {
        if (!_settings.Current.ShowToastNotifications)
        {
            return;
        }

        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
        catch
        {
            try
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    TrayNotification?.Invoke(title, message);
                });
            }
            catch
            {
                // best-effort
            }
        }
    }

    public event Action<string, string>? TrayNotification;
}

internal static class NativeMethods
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int SetCurrentProcessExplicitAppUserModelID(string appID);
}

internal static class ShortcutHelper
{
    private static readonly PropertyKey AppUserModelIdKey = new(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);

    public static void Ensure(string aumid, string name)
    {
        var programs = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        Directory.CreateDirectory(programs);
        var path = Path.Combine(programs, $"{name}.lnk");
        var exe = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exe) || File.Exists(path))
        {
            return;
        }

        var link = (IShellLinkW)new CShellLink();
        link.SetPath(exe);
        link.SetWorkingDirectory(Path.GetDirectoryName(exe)!);
        link.SetDescription(name);

        var store = (IPropertyStore)link;
        var key = AppUserModelIdKey;
        using var variant = new PropVariant(aumid);
        store.SetValue(ref key, variant);
        store.Commit();
        ((IPersistFile)link).Save(path, true);
        Marshal.ReleaseComObject(link);
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    private class CShellLink;

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    private interface IShellLinkW
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] char[] pszFile, int cchMaxPath, IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] char[] pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] char[] pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] char[] pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] char[] pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
    private interface IPropertyStore
    {
        void GetCount(out uint cProps);
        void GetAt(uint iProp, out PropertyKey pkey);
        void GetValue(ref PropertyKey key, out PropVariant pv);
        void SetValue(ref PropertyKey key, PropVariant pv);
        void Commit();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private readonly struct PropertyKey(Guid formatId, uint propertyId)
    {
        public readonly Guid FormatId = formatId;
        public readonly uint PropertyId = propertyId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private sealed class PropVariant : IDisposable
    {
        private ushort _vt;
        private ushort _wReserved1;
        private ushort _wReserved2;
        private ushort _wReserved3;
        private IntPtr _p;
        private int _p2;

        public PropVariant(string value)
        {
            _vt = 31; // VT_LPWSTR
            _p = Marshal.StringToCoTaskMemUni(value);
        }

        public void Dispose()
        {
            PropVariantClear(this);
            GC.SuppressFinalize(this);
        }

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear([In, Out] PropVariant pvar);
    }
}
