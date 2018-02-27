using System.Threading;

namespace KakaoTalkBot.Utilities
{
    public static class MouseUtilities
    {
        public static void Move(int x, int y) => Win32.SetCursorPos(x, y);
        public static void Click(int x, int y) => Win32.MouseEvent(Win32.MOUSEEVENTF_LEFTDOWN | Win32.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

        public static void MoveAndClick(int x, int y)
        {
            Move(x, y);
            Click(x, y);
        }

        public static void MoveAndClickAndPaste(int x, int y, string text, int timeout)
        {
            MoveAndClick(x, y);
            Click(x, y);
            ClipboardUtilities.Paste(text);
            Thread.Sleep(timeout);
        }
    }
}
