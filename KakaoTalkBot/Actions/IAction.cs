using Emgu.CV;
using BotLibrary.Utilities;

namespace KakaoTalkBot.Actions
{
    public interface IAction
    {
        bool IsCompleted { get; set; }
        string CurrentActionName { get; }
        event BaseAction.TextDelegate NewLog;
        ApplicationInfo Info { get; set; }

        bool OnAction(Mat mat);

        int PasteTimeout { get; set; }
        int FieldClickTimeout { get; set; }
    }
}
