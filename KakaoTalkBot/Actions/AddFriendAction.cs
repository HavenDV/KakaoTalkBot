using System;
using System.Threading;
using Emgu.CV;
using KakaoTalkBot.Utilities;

namespace KakaoTalkBot.Actions
{
    public class AddFriendAction : BaseAction
    {
        public enum CurrentAction
        {
            Started,
            AddByContacts,
            CountryCode,
            SearchCountryCode,
            AddOk,
            AddedCancel,
            CheckAlreadyAdded,
            NeedBack,
            Unknown
        }

        public CurrentAction Action { get; set; } = CurrentAction.Started;
        public override string CurrentActionName => Action.ToString("G");

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        private void AddByContacts(IInputArray mat)
        {
            if (!IsExists(mat, "add_by_contacts_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "add_by_contacts_nox.bmp");
            x += w / 2;
            y += 3 * h / 4;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);
        }

        private void CountryCode(IInputArray mat)
        {
            if (!IsExists(mat, "add_by_contacts_phone_code_nox.bmp"))
            {
                return;
            }

            ClipboardUtilities.Paste(Name);

            Thread.Sleep(500);

            var (x, y, w, h) = Find(mat, "add_by_contacts_phone_code_nox.bmp");
            x += w / 2;
            y += h / 2;

            Thread.Sleep(500);

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);
        }

        private void SearchCountryCode(IInputArray mat)
        {
            if (!IsExists(mat, "search_phone_code_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "search_phone_code_nox.bmp");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            ClipboardUtilities.Paste(Country);

            Thread.Sleep(1500);

            y += 6 * h;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(1500);

            ClipboardUtilities.Paste(Phone);

            Thread.Sleep(1500);
        }

        private void AddOk(IInputArray mat)
        {
            if (!IsExists(mat, "add_ok_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "add_ok_nox.bmp");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);
        }

        private void AddedCancel(IInputArray mat)
        {
            if (!IsExists(mat, "added_cancel_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "added_cancel_nox.bmp");
            x += w / 4;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);
        }

        private void AlreadyAdded(IInputArray mat)
        {
            if (!IsExists(mat, "already_added_ok.bmp"))
            {
                Action = CurrentAction.Started;
                return;
            }

            var (x, y, w, h) = Find(mat, "already_added_ok.bmp");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);

            Action = CurrentAction.NeedBack;
        }

        private void NeedBack(IInputArray mat)
        {
            if (!IsExists(mat, "back_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find(mat, "back_nox.bmp");
            x += w / 2;
            y += h / 2;

            MouseUtilities.MoveAndClick(x, y);

            Thread.Sleep(500);
        }

        public override bool OnAction(IInputArray mat)
        {
            switch (Action)
            {
                case CurrentAction.Started:
                    AddByContacts(mat);
                    Action = CurrentAction.AddByContacts;
                    return true;

                case CurrentAction.AddByContacts:
                    CountryCode(mat);
                    Action = CurrentAction.CountryCode;
                    return true;

                case CurrentAction.CountryCode:
                    SearchCountryCode(mat);
                    Action = CurrentAction.SearchCountryCode;
                    return true;

                case CurrentAction.SearchCountryCode:
                    AddOk(mat);
                    Action = CurrentAction.AddOk;
                    return true;

                case CurrentAction.AddOk:
                    AddOk(mat);
                    Action = CurrentAction.AddedCancel;
                    return true;

                case CurrentAction.AddedCancel:
                    AddedCancel(mat);
                    Action = CurrentAction.CheckAlreadyAdded;
                    return true;

                case CurrentAction.CheckAlreadyAdded:
                    AlreadyAdded(mat);
                    IsCompleted = Action == CurrentAction.Started;
                    return true;

                case CurrentAction.NeedBack:
                    NeedBack(mat);
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
