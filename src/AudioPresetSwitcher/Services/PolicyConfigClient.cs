using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace AudioPresetSwitcher.Services;

internal static class PolicyConfigClient
{
    public static void SetDefaultEndpoint(string deviceId, Role role)
    {
        var client = new CPolicyConfigClient();
        var policy = client as IPolicyConfig;
        if (policy is not null)
        {
            Marshal.ThrowExceptionForHR(policy.SetDefaultEndpoint(deviceId, role));
            return;
        }

        var vista = client as IPolicyConfigVista;
        if (vista is not null)
        {
            Marshal.ThrowExceptionForHR(vista.SetDefaultEndpoint(deviceId, role));
            return;
        }

        var win10 = client as IPolicyConfig10;
        if (win10 is not null)
        {
            Marshal.ThrowExceptionForHR(win10.SetDefaultEndpoint(deviceId, role));
            return;
        }

        throw new InvalidOperationException("Unable to create the Windows audio policy client.");
    }

    [ComImport]
    [Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    private class CPolicyConfigClient
    {
    }

    [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPolicyConfig
    {
        [PreserveSig] int Unused1();
        [PreserveSig] int Unused2();
        [PreserveSig] int Unused3();
        [PreserveSig] int Unused4();
        [PreserveSig] int Unused5();
        [PreserveSig] int Unused6();
        [PreserveSig] int Unused7();
        [PreserveSig] int Unused8();
        [PreserveSig] int Unused9();
        [PreserveSig] int Unused10();

        [PreserveSig]
        int SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string deviceId, Role role);

        [PreserveSig] int Unused12();
    }

    [Guid("568B9108-44BF-40B4-9006-86AFE5B5A620")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPolicyConfigVista
    {
        [PreserveSig] int Unused1();
        [PreserveSig] int Unused2();
        [PreserveSig] int Unused3();
        [PreserveSig] int Unused4();
        [PreserveSig] int Unused5();
        [PreserveSig] int Unused6();
        [PreserveSig] int Unused7();
        [PreserveSig] int Unused8();
        [PreserveSig] int Unused9();
        [PreserveSig] int Unused10();

        [PreserveSig]
        int SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string deviceId, Role role);

        [PreserveSig] int Unused12();
    }

    [Guid("00000000-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPolicyConfig10
    {
        [PreserveSig] int Unused1();
        [PreserveSig] int Unused2();
        [PreserveSig] int Unused3();
        [PreserveSig] int Unused4();
        [PreserveSig] int Unused5();
        [PreserveSig] int Unused6();
        [PreserveSig] int Unused7();
        [PreserveSig] int Unused8();
        [PreserveSig] int Unused9();
        [PreserveSig] int Unused10();

        [PreserveSig]
        int SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string deviceId, Role role);

        [PreserveSig] int Unused12();
    }
}
