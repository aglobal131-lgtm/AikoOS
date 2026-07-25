using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AikoOS.Behavior.Idle;

public sealed class WindowsUserIdleDetector : IUserIdleDetector
{
    public TimeSpan GetIdleDuration()
    {
        LASTINPUTINFO lastInputInfo = new()
        {
            Size = (uint)Marshal.SizeOf<LASTINPUTINFO>()
        };

        if (!GetLastInputInfo(ref lastInputInfo))
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error());
        }

        ulong currentTickCount =
            GetTickCount64();

        ulong lastInputTickCount =
            lastInputInfo.Time;

        ulong idleMilliseconds =
            currentTickCount - lastInputTickCount;

        return TimeSpan.FromMilliseconds(
            idleMilliseconds);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetLastInputInfo(
        ref LASTINPUTINFO lastInputInfo);

    [DllImport("kernel32.dll")]
    private static extern ulong GetTickCount64();

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint Size;

        public uint Time;
    }
}