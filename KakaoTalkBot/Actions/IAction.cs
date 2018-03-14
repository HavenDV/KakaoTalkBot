using Emgu.CV;
using BotLibrary.Utilities;

namespace KakaoTalkBot.Actions
{
    public interface IAction
    {
        bool IsCompleted { get; set; }
        string CurrentActionName { get; }
        event BaseAction.TextDelegate NewLog;
        Screens Screens { get; set; }

        bool OnAction(Mat mat);
    }
}
