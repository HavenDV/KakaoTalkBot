using System;
using Emgu.CV;
using System.Collections.Generic;
using System.Threading;
using KakaoTalkBot.Utilities;
using MatUtilities = KakaoTalkBotLibrary.Utilities.MatUtilities;

namespace KakaoTalkBot.Actions
{
    public abstract class BaseAction : IAction
    {
        public IInputArray Mat { get; set; }
        public bool IsCompleted { get; set; }

        public delegate void TextDelegate(string text);
        public event TextDelegate NewLog;
        protected void Log(string text) => NewLog?.Invoke(text);

        public Dictionary<string, Mat> AnchorsDictionary { get; set; } = new Dictionary<string, Mat>();
        protected Mat GetAnchor(string name) => AnchorsDictionary.TryGetValue(name, out var result)
            ? result : throw new Exception($"Anchor {name} is not found");

        protected (int, int, int, int) Find(string name)
        {
            return MatUtilities.Find(Mat, GetAnchor(name));
        }

        protected bool IsExists(string name)
        {
            return MatUtilities.IsExists(Mat, GetAnchor(name));
        }

        protected static void Sleep(int millisecondsTimeout) => Thread.Sleep(millisecondsTimeout);
        protected static void MoveAndClick(int x, int y, int dx = 0, int dy = 0) => MouseUtilities.MoveAndClick(x + dx, y + dy);

        protected static void Paste(string text, int sleep)
        {
            ClipboardUtilities.Paste(text);
            Sleep(sleep);
        }

        public bool OnAction(IInputArray mat)
        {
            try
            {
                Mat = mat;

                return OnActionInternal(mat);
            }
            finally
            {
                Sleep(500);
            }
        }

        protected abstract bool OnActionInternal(IInputArray mat);
        public abstract string CurrentActionName { get; }
    }
}
