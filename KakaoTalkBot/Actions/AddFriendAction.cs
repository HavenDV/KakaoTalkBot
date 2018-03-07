using System;
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

        private void AddByContacts()
        {
            if (!IsExists("add_by_contacts_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find("add_by_contacts_nox.bmp");
            MoveAndClick(x, y, w / 2, 3 * h / 4);
        }

        private void CountryCode()
        {
            if (!IsExists("add_by_contacts_phone_code_nox.bmp"))
            {
                return;
            }

            Paste(Name, 500);

            var (x, y, w, h) = Find("add_by_contacts_phone_code_nox.bmp");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        private void SearchCountryCode()
        {
            if (!IsExists("search_phone_code_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find("search_phone_code_nox.bmp");

            x += w / 2;
            y += h / 2;
            MouseUtilities.MoveAndClick(x, y);

            Paste(Country, 500);

            y += 6 * h;
            MouseUtilities.MoveAndClick(x, y);

            Sleep(500);

            Paste(Phone, 500);
        }

        private void AddOk()
        {
            if (!IsExists("add_ok_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find("add_ok_nox.bmp");
            MoveAndClick(x, y, w / 2, h / 2);

            Sleep(3000);
        }

        private void AddedCancel()
        {
            if (!IsExists("added_cancel_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find("added_cancel_nox.bmp");
            MoveAndClick(x, y, w / 4, h / 2);
        }

        private void AlreadyAdded()
        {
            if (!IsExists("already_added_ok.bmp") && !IsExists("add_ok_nox.bmp"))
            {
                Action = CurrentAction.Started;
                return;
            }

            var (x, y, w, h) = Find("already_added_ok.bmp");
            MoveAndClick(x, y, w / 2, h / 2);

            Action = CurrentAction.NeedBack;
        }

        private void NeedBack()
        {
            if (!IsExists("back_nox.bmp"))
            {
                return;
            }

            var (x, y, w, h) = Find("back_nox.bmp");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        protected override bool OnActionInternal(IInputArray mat)
        {
            switch (Action)
            {
                case CurrentAction.Started:
                    AddByContacts();
                    Action = CurrentAction.AddByContacts;
                    return true;

                case CurrentAction.AddByContacts:
                    CountryCode();
                    Action = CurrentAction.CountryCode;
                    return true;

                case CurrentAction.CountryCode:
                    SearchCountryCode();
                    Action = CurrentAction.SearchCountryCode;
                    return true;

                case CurrentAction.SearchCountryCode:
                    AddOk();
                    Action = CurrentAction.AddOk;
                    return true;

                case CurrentAction.AddOk:
                    AddOk();
                    Action = CurrentAction.AddedCancel;
                    return true;

                case CurrentAction.AddedCancel:
                    AddedCancel();
                    Action = CurrentAction.CheckAlreadyAdded;
                    return true;

                case CurrentAction.CheckAlreadyAdded:
                    AlreadyAdded();
                    IsCompleted = Action == CurrentAction.Started;
                    return true;

                case CurrentAction.NeedBack:
                    NeedBack();
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
