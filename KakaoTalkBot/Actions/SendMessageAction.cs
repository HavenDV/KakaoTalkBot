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
            SendMessagePrepare,
            SendMessage,
            Unknown
        }

        public CurrentAction Action { get; set; } = CurrentAction.Started;
        public override string CurrentActionName => Action.ToString("G");

        public string Text { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        private void Search()
        {
            if (!IsExists("search.bmp", "SearchField"))
            {
                return;
            }

            var (x, y, w, h) = Find("search.bmp", "SearchField");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Sleep(1000);

            Paste(Phone, 500);

            y += 2 * h;
            MouseUtilities.MoveAndClick(x, y);
        }

        private void Chat()
        {
            if (!IsExists("profile.bmp", "FreeChat"))
            {
                return;
            }

            var (x, y, w, h) = Find("profile.bmp", "FreeChat");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        private void SendMessagePrepare()
        {
            if (!IsExists("chat.bmp", "ChatField"))
            {
                return;
            }

            var (x, y, w, h) = Find("chat.bmp", "ChatField");
            MoveAndClick(x, y, w / 2, h / 2);

            Sleep(500);

            Paste(Text, 500);
        }

        private void SendMessage()
        {
            if (!IsExists("chat_beforesend.bmp", "Back"))
            {
                return;
            }

            var (x, y, w, h) = Find("chat_beforesend.bmp", "SendButton");
            MoveAndClick(x, y, w / 2, h / 2);

            Sleep(500);

            (x, y, w, h) = Find("chat_beforesend.bmp", "Back");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        protected override bool OnActionInternal(Mat mat)
        {
            switch (Action)
            {
                case CurrentAction.Started:
                    Search();
                    Action = CurrentAction.Profile;
                    return true;

                case CurrentAction.Profile:
                    Chat();
                    Action = CurrentAction.Chat;
                    return true;

                case CurrentAction.Chat:
                    SendMessagePrepare();
                    Action = CurrentAction.SendMessagePrepare;
                    return true;

                case CurrentAction.SendMessagePrepare:
                    SendMessage();
                    Action = CurrentAction.SendMessage;
                    return true;

                case CurrentAction.SendMessage:
                    Action = CurrentAction.Started;
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
