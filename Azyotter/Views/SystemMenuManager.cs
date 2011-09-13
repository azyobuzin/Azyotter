using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Azyobuzi.Azyotter.Views
{
    public static class SystemMenuManager
    {
        [DllImport("USER32.DLL")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("USER32.DLL")]
        private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

        private const uint MF_STRING = 0x00000000;
        private const uint MF_SEPARATOR = 0x00000800;
        private const int WM_SYSCOMMAND = 0x112;
        
        public static void AddSeparator(Window target)
        {
            var source = (HwndSource)HwndSource.FromVisual(target);
            var hMenu = GetSystemMenu(source.Handle, false);
            AppendMenu(hMenu, MF_SEPARATOR, 0, string.Empty);
        }

        public static void AddMenuItem(Window target, uint id, string text, Action onClick)
        {
            var source = (HwndSource)HwndSource.FromVisual(target);
            var hMenu = GetSystemMenu(source.Handle, false);
            AppendMenu(hMenu, MF_STRING, id, text);
            source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                if (msg == WM_SYSCOMMAND && wParam.ToInt32() == id)
                {
                    onClick();
                    handled = true;
                }

                return IntPtr.Zero;
            });
        }
    }
}
