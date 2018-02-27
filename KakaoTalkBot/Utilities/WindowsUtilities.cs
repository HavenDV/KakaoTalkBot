using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace KakaoTalkBot.Utilities
{
    public static class WindowsUtilities
    {
        public static void ShowWindow(string name, int timeout)
        {
            var processes = Process.GetProcessesByName(name).Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));
            var process = processes.FirstOrDefault();
            if (process == null)
            {
                return;
            }

            var ptr = new HandleRef(null, process.MainWindowHandle);
            const int swShow = 5;
            Win32.ShowWindow(ptr, swShow);
            Win32.SetForegroundWindow(ptr);
            Win32.SetFocus(ptr);

            Thread.Sleep(timeout);
        }
    }
}
