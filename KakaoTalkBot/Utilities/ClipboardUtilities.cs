using System.Threading;
using System.Windows;

namespace KakaoTalkBot.Utilities
{
    public static class ClipboardUtilities
    {
        public static void ClipboardCommand(string command)
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
        }

        public static void Paste(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            ClipboardCommand(command);
            KeyboardUtilities.KeyboardCommand("Control+V");
        }
    }
}
