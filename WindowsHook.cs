using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace bucklespring.net
{
    public class WindowsHook : IDisposable
    {
        private const String User32Dll = "user32.dll";
        private const String Kernel32Dll = "kernel32.dll";

        [DllImport(User32Dll, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId
        );

        [DllImport(User32Dll, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(
            IntPtr hhk
        );

        [DllImport(User32Dll, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport(Kernel32Dll, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(
            string lpModuleName
        );

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private enum EWindowsHook
        {
            KEYBOARD_LL = 0x0D //13
        }

        private enum EWindowsMessage
        {
            KEYDOWN = 0x0100,
            SYSKEYDOWN = 0x0104
        }

        private LowLevelKeyboardProc _LowLevelKeyboardProc;
        private IntPtr HookID = IntPtr.Zero;
        
        public delegate void KeyboardEventHandler(Int32 vkCode, Boolean IsDown);
        public event KeyboardEventHandler OnKeyboardEvent;

        public WindowsHook()
        {
            _LowLevelKeyboardProc = HookCallback;
            InitializeHook();
        }
        
        private void InitializeHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                HookID = SetWindowsHookEx((Int32)EWindowsHook.KEYBOARD_LL, _LowLevelKeyboardProc, GetModuleHandle(curModule.ModuleName), 0);
            }

        }
        
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Boolean IsDown = wParam == (IntPtr)EWindowsMessage.KEYDOWN || wParam == (IntPtr)EWindowsMessage.SYSKEYDOWN;
            int vkCode = Marshal.ReadInt32(lParam);

            OnKeyboardEvent?.Invoke(vkCode, IsDown);

            return CallNextHookEx(HookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(HookID);
        }
    }
}
