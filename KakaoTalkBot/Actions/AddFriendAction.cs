using System;

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
            if (!IsExists("find_subtab.bmp", "Menu", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.75);
        }

        private void CountryCode()
        {
            if (!IsExists("add_by_contacts.bmp", "SelectCode", out var tuple))
            {
                return;
            }

            Paste(Name);
            MoveAndClick(tuple, 0.5, 0.5);
        }

        private void SearchCountryCode()
        {
            if (!IsExists("select_phone_code_normal.bmp", "SearchField", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            Sleep(FieldClickTimeout);

            Paste(Country);
        }

        private void SearchCountryCodeAfterPaste()
        {
            if (!IsExists("select_phone_code_afterpaste.bmp", "Results", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            Sleep(FieldClickTimeout);

            Paste(Phone);
        }

        private void AddOk()
        {
            if (!IsExists("add_by_contacts_beforeok.bmp", "OK", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);
        }

        private void AddedCancel()
        {
            if (!IsExists("add_by_contacts_success.bmp", "Cancel", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.25, 0.5);
        }

        private void AlreadyAdded()
        {
            if (!IsExists("add_by_contacts_already.bmp", "OK", out var tuple) && !IsExists("add_by_contacts_beforeok.bmp", "OK", out var _))
            {
                Action = CurrentAction.Started;
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);

            Action = CurrentAction.NeedBack;
        }

        private void NeedBack()
        {
            if (!IsExists("add_by_contacts.bmp", "Back", out var tuple))
            {
                return;
            }

            MoveAndClick(tuple, 0.5, 0.5);
        }

        protected override bool OnActionInternal()
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
