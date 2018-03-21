using System;
using System.Diagnostics;

namespace KakaoTalkBot.Actions
{
    public class SendMessageAction : BaseAction
    {
        public enum CurrentAction
        {
            Started,
            Profile,
            Chat,
            SendMessagePrepare,
            SendMessage,
            SendMessageAfter,
            Completed,
            Unknown
        }

        public CurrentAction Action { get; set; } = CurrentAction.Started;
        public override string CurrentActionName => Action.ToString("G");

        public string Text { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        private void Search()
        {
            if (!IsExists("search.bmp", "SearchField", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            Sleep(FieldClickTimeout);

            Paste(Phone);

            MoveAndClick(tuple, 0.5, 2.5);
        }

        private void Chat()
        {
            if (!IsExists("profile.bmp", "FreeChat", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 1.0 / 6, 0.5);
        }

        private bool SendMessagePrepare()
        {
            if (!IsExists("chat.bmp", "ChatField", out var tuple))
            {
                return false;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            Sleep(FieldClickTimeout);

            Paste(Text);

            return true;
        }

        private void SendMessage()
        {
            if (!IsExists("chat_beforesend.bmp", "SendButton", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);

        }

        private bool SendMessageAfterIfMessage()
        {
            if (!IsExists("char_aftersend_measures.bmp", "OK", out var tuple))
            {
                return false;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            return true;
        }

        private void SendMessageAfter()
        {
            if (!IsExists("chat_beforesend.bmp", "Back", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);
        }

        protected override bool OnActionInternal()
        {
            var watch = Stopwatch.StartNew();
            try
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
                        if (!SendMessagePrepare())
                        {
                            goto case CurrentAction.SendMessagePrepare;
                        }
                        Action = CurrentAction.SendMessagePrepare;
                        return true;

                    case CurrentAction.SendMessagePrepare:
                        SendMessage();
                        Action = CurrentAction.SendMessage;
                        return true;

                    case CurrentAction.SendMessage:
                        if (!SendMessageAfterIfMessage())
                        {
                            goto case CurrentAction.SendMessageAfter;
                        }
                        Action = CurrentAction.SendMessageAfter;
                        return true;

                    case CurrentAction.SendMessageAfter:
                        SendMessageAfter();
                        IsCompleted = true;
                        Action = CurrentAction.Completed;
                        return true;

                    case CurrentAction.Unknown:
                        Log("Unknown window. Stop");
                        return false;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            finally
            {
                watch.Stop();
                //MessageBox.Show($"Test: {watch.ElapsedMilliseconds}");
            }

        }
    }
}
