using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using Emgu.CV;
using Emgu.CV.CvEnum;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using Pranas;

namespace KakaoTalkBot
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static string GetAnchor(string name) => Path.Combine("..", "..", "..", "anchors", name);

        private void Log(string text) => LogTextBox.Text += Environment.NewLine + text;

        private static (int, int, int, int) Find(IInputArray mat, Mat obj)
        {
            using (var resultMat = new Mat())
            {
                CvInvoke.MatchTemplate(mat, obj, resultMat, TemplateMatchingType.Sqdiff);
                resultMat.MinMax(out _, out _, out var points, out _);
                var x = points[0].X;
                var y = points[0].Y;

                return (x, y, obj.Width, obj.Height);
            }
        }

        private static (int, int, int, int) Find(IInputArray mat, string name)
        {
            using (var obj = new Mat(GetAnchor(name)).ToGray())
            {
                return Find(mat, obj);
            }
        }

        private static void MoveMouse(int x, int y) => Win32.SetCursorPos(x, y);
        private static void ClickMouse(int x, int y) => Win32.MouseEvent(Win32.MOUSEEVENTF_LEFTDOWN | Win32.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

        private static void MoveAndClick(int x, int y)
        {
            MoveMouse(x, y);
            ClickMouse(x, y);
        }

        private static void MoveAndClickAndPaste(int x, int y, string text, int timeout)
        {
            MoveAndClick(x, y);
            ClickMouse(x, y);
            Paste(text);
            Thread.Sleep(timeout);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(HandleRef hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(HandleRef hwnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(HandleRef hwnd);

        private static void ShowWindow(string name, int timeout)
        {
            var processes = Process.GetProcessesByName(name).Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));
            var process = processes.FirstOrDefault();
            if (process == null)
            {
                return;
            }

            var ptr = new HandleRef(null, process.MainWindowHandle);
            const int swShow = 5;
            ShowWindow(ptr, swShow);
            SetForegroundWindow(ptr);
            SetFocus(ptr);

            Thread.Sleep(timeout);
        }

        private static void ClipboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetText(command);
                    return;
                }
                catch
                {
                    // ignored
                }

                Thread.Sleep(10);
            }
            //Clipboard.SetText(command);
        }

        private static VirtualKeyCode ToVirtualKey(string key)
        {
            string text;
            if (key.Length == 1)
            {
                text = $"VK_{key}";
            }
            else if (key.ToLowerInvariant() == "alt")
            {
                text = "MENU";
            }
            else if (key.ToLowerInvariant() == "ctrl")
            {
                text = "CONTROL";
            }
            else if (key.ToLowerInvariant() == "enter")
            {
                text = "RETURN";
            }
            else
            {
                text = key;
            }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), text, true);
        }

        private static void KeyboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var keys = command.Split('+');
            var mainKey = ToVirtualKey(keys.LastOrDefault());
            var otherKeys = keys.Take(keys.Length - 1).Select(ToVirtualKey);

            new InputSimulator().Keyboard.ModifiedKeyStroke(otherKeys, mainKey);
        }

        private static void Paste(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            ClipboardCommand(command);
            KeyboardCommand("Control+V");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowWindow("KakaoTalk", 100);

                using (var image = ScreenshotCapture.TakeScreenshot(true))
                using (var bitmap = new Bitmap(image))
                using (var mat = bitmap.ToMat().ToGray())
                {
                    var (x, y, w, h) = Find(mat, "auth_logo.bmp");
                    x += w / 2;
                    y += h + 20;

                    MoveAndClickAndPaste(x, y, EmailTextBox.Text, 50);
                    y += 40;

                    MoveAndClickAndPaste(x, y, PasswordTextBox.Text, 50);
                    y += 40;

                    MoveAndClick(x, y);
                }

                Log("Auth completed");
            }
            catch (Exception exception)
            {
                Log($"Exception: {exception}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (var image = ScreenshotCapture.TakeScreenshot(true))
                using (var bitmap = new Bitmap(image))
                using (var _ = bitmap.ToMat().ToGray())
                {
                }
            });
        }
    }
}
