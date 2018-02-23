using System.Runtime.InteropServices;

namespace KakaoTalkBot.Utilities
{
    public class Win32
    {
        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "mouse_event", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void MouseEvent(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
    }
}
