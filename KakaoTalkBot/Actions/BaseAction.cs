using Emgu.CV;
using KakaoTalkBot.Utilities;
using System.Collections.Generic;
using System.Threading;

namespace KakaoTalkBot.Actions
{
    public abstract class BaseAction : IAction
    {
        public bool IsCompleted { get; set; }

        public delegate void TextDelegate(string text);
        public event TextDelegate NewLog;
        protected void Log(string text) => NewLog?.Invoke(text);

        public Dictionary<string, Mat> AnchorsDictionary { get; set; } = new Dictionary<string, Mat>();
        protected Mat GetAnchor(string name) => AnchorsDictionary.TryGetValue(name, out var result) ? result : null;

        protected (int, int, int, int) Find(IInputArray mat, string name)
        {
            return MatUtilities.Find(mat, GetAnchor(name));
        }

        protected bool IsExists(IInputArray mat, string name)
        {
            return MatUtilities.IsExists(mat, GetAnchor(name));
        }

        protected void Sleep(int millisecondsTimeout) => Thread.Sleep(millisecondsTimeout);

        public abstract bool OnAction(IInputArray mat);
        public abstract string CurrentActionName { get; }
    }
}
