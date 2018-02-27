using System.Threading;

namespace KakaoTalkBot.Utilities
{
    public static class MouseUtilities
    {
        public static void MoveMouse(int x, int y) => Win32.SetCursorPos(x, y);
        public static void ClickMouse(int x, int y) => Win32.MouseEvent(Win32.MOUSEEVENTF_LEFTDOWN | Win32.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

        public static void MoveAndClick(int x, int y)
        {
            MoveMouse(x, y);
            ClickMouse(x, y);
        }

        public static void MoveAndClickAndPaste(int x, int y, string text, int timeout)
        {
            MoveAndClick(x, y);
            ClickMouse(x, y);
            ClipboardUtilities.Paste(text);
            Thread.Sleep(timeout);
        }
    }
}
