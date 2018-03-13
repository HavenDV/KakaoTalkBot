using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace BotLibrary.Utilities
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
            //const int swShow = 5;
            const int swRestore = 9;
            Win32.ShowWindow(ptr, swRestore);
            Win32.SetForegroundWindow(ptr);
            Win32.SetFocus(ptr);

            Thread.Sleep(timeout);
        }

        public static (Win32.Rect, Bitmap) GetScreenshotOfWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return (new Win32.Rect(), null);
            }

            Win32.GetWindowRect(handle, out var rect);
            if (rect.Right < 0 || rect.Bottom < 0)
            {
                return (new Win32.Rect(), null);
            }

            var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);

            //var hdc = graphics.GetHdc();
            //Win32.PrintWindow(handle, hdc, 0);
            //graphics.ReleaseHdc(hdc);
            
            graphics.Dispose();

            return (rect, bitmap);
        }

        public static (Win32.Rect, Bitmap)[] GetScreenshotOfProcess(string name) => Process
            .GetProcessesByName(name)
            .Select(i => GetScreenshotOfWindow(i.MainWindowHandle))
            .Where(i => i.Item2 != null)
            .ToArray();
    }
}
