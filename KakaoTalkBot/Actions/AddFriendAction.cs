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
            SearchCountryCodeAfterPaste,
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
            if (!IsExists("find_subtab.bmp", "Menu"))
            {
                return;
            }

            var (x, y, w, h) = Find("find_subtab.bmp", "Menu");
            MoveAndClick(x, y, w / 2, 3 * h / 4);
        }

        private void CountryCode()
        {
            if (!IsExists("add_by_contacts.bmp", "SelectCode"))
            {
                return;
            }

            Paste(Name, 500);

            var (x, y, w, h) = Find("add_by_contacts.bmp", "SelectCode");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        private void SearchCountryCode()
        {
            if (!IsExists("select_phone_code_normal.bmp", "SearchField"))
            {
                return;
            }

            var (x, y, w, h) = Find("select_phone_code_normal.bmp", "SearchField");

            x += w / 2;
            y += h / 2;
            MouseUtilities.MoveAndClick(x, y);

            Paste(Country, 500);
        }

        private void SearchCountryCodeAfterPaste()
        {
            if (!IsExists("select_phone_code_afterpaste.bmp", "Results"))
            {
                return;
            }

            var (x, y, w, h) = Find("select_phone_code_afterpaste.bmp", "Results");

            x += w / 2;
            y += 3 * h / 4;
            MouseUtilities.MoveAndClick(x, y);

            Sleep(500);

            Paste(Phone, 500);
        }

        private void AddOk()
        {
            if (!IsExists("add_by_contacts_beforeok.bmp", "OK"))
            {
                return;
            }

            var (x, y, w, h) = Find("add_by_contacts_beforeok.bmp", "OK");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        private void AddedCancel()
        {
            if (!IsExists("add_by_contacts_success.bmp", "Cancel"))
            {
                return;
            }

            var (x, y, w, h) = Find("add_by_contacts_success.bmp", "Cancel");
            MoveAndClick(x, y, w / 4, h / 2);
        }

        private void AlreadyAdded()
        {
            if (!IsExists("add_by_contacts_already.bmp", "OK") && !IsExists("add_by_contacts_beforeok.bmp", "OK"))
            {
                Action = CurrentAction.Started;
                return;
            }

            var (x, y, w, h) = Find("add_by_contacts_already.bmp", "OK");
            MoveAndClick(x, y, w / 2, h / 2);

            Action = CurrentAction.NeedBack;
        }

        private void NeedBack()
        {
            if (!IsExists("add_by_contacts.bmp", "Back"))
            {
                return;
            }

            var (x, y, w, h) = Find("add_by_contacts.bmp", "Back");
            MoveAndClick(x, y, w / 2, h / 2);
        }

        protected override bool OnActionInternal(Mat mat)
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
                    SearchCountryCodeAfterPaste();
                    Action = CurrentAction.SearchCountryCodeAfterPaste;
                    return true;

                case CurrentAction.SearchCountryCodeAfterPaste:
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
