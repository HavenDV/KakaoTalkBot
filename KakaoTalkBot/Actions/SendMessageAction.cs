using System;
using Emgu.CV;
using KakaoTalkBot.Utilities;

namespace KakaoTalkBot.Actions
{
    public class SendMessageAction : BaseAction
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
        public override string CurrentActionName => Action.ToString("G");

        public string Text { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

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

            Sleep(1000);

            ClipboardUtilities.Paste(Phone);

            Sleep(1000);

            y += (2 * h);
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

            Sleep(1000);

            MouseUtilities.MoveAndClick(x, y);
        }

        public override bool OnAction(IInputArray mat)
        {
            switch (Action)
            {
                case CurrentAction.Started:
                    Search(mat);
                    Action = CurrentAction.Profile;
                    return true;

                case CurrentAction.Profile:
                    Chat(mat);
                    Action = CurrentAction.Chat;
                    return true;

                case CurrentAction.Chat:
                    Action = CurrentAction.Completed;
                    IsCompleted = true;
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
