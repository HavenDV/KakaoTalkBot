using Emgu.CV;
using System.Collections.Generic;

namespace KakaoTalkBot.Actions
{
    public interface IAction
    {
        bool IsCompleted { get; set; }
        string CurrentActionName { get; }
        event BaseAction.TextDelegate NewLog;
        Dictionary<string, Mat> AnchorsDictionary { get; set; }

        bool OnAction(IInputArray mat);
    }
}
