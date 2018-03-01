using System;
using System.Collections.Generic;
using System.Threading;
using Emgu.CV;
using KakaoTalkBot.Utilities;

namespace KakaoTalkBot.Actions
{
    public class SendMessageAction
    {
        public enum CurrentAction
        {
            Started,
            Search,
            Profile,
            Chat,
            Completed,
            Unknown
        }

        public CurrentAction Action { get; set; } = CurrentAction.Started;

        public string Text { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public delegate void TextDelegate(string text);
        public event TextDelegate NewLog;
        private void Log(string text) => NewLog?.Invoke(text);

        public Dictionary<string, Mat> AnchorsDictionary { get; set; } = new Dictionary<string, Mat>();
        private Mat GetAnchor(string name) => AnchorsDictionary.TryGetValue(name, out var result) ? result : null;

        private (int, int, int, int) Find(IInputArray mat, string name)
        {
            return MatUtilities.Find(mat, GetAnchor(name));
        }

        private bool IsExists(IInputArray mat, string name)
        {
            return MatUtilities.IsExists(mat, GetAnchor(name));
        }

        private void Search(IInputArray mat)
        {
            if (!IsExists(mat, "nox_search.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "nox_search.bmp");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(1000);

            ClipboardUtilities.Paste(Phone);

            Thread.Sleep(1000);

            y += (int)(2 * h);
            MouseUtilities.MoveAndClick(x, y);
        }

        private void Chat(IInputArray mat)
        {
            if (!IsExists(mat, "nox_free_chat.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "nox_free_chat.bmp");
            x += w / 2;
            y += h / 2;

            Thread.Sleep(1000);

            MouseUtilities.MoveAndClick(x, y);

            Action = CurrentAction.Chat;
        }

        public bool OnAction(IInputArray mat)
        {
            switch (Action)
            {
                case CurrentAction.Started:
                    Search(mat);
                    Action = CurrentAction.Profile;
                    return true;

                case CurrentAction.Profile:
                    Chat(mat);
                    return true;

                case CurrentAction.Chat:
                    Action = CurrentAction.Completed;
                    return true;

                case CurrentAction.Unknown:
                    Log("Unknown window. Stop");
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
