using System;
using Emgu.CV;
using System.Threading;
using KakaoTalkBot.Utilities;
using BotLibrary.Utilities;
using Emgu.CV.Structure;

namespace KakaoTalkBot.Actions
{
    public abstract class BaseAction : IAction
    {
        public Mat Mat { get; set; }
        public bool IsCompleted { get; set; }
        public bool EnableLog { get; set; }

        public delegate void TextDelegate(string text);
        public event TextDelegate NewLog;
        protected void Log(string text) => NewLog?.Invoke(text);

        public Screens Screens { get; set; }
        protected Mat GetAnchor(string screenName, string anchorName) => 
            Screens.GetAnchor(screenName, anchorName, Mat.Size) ??
            throw new Exception($"Anchor {anchorName} is not found on screen {screenName}");

        protected void Log(Action<Mat> action, string prefix)
        {
            if (!EnableLog)
            {
                return;
            }

            using (var mat = Mat.Clone())
            {
                action?.Invoke(mat);
                mat.Save($"D:/logs/log_{prefix}_{new Random().Next()}.png");
            }
        }

        protected (int x, int y, int w, int h) Find(string screenName, string anchorName)
        {
            var (rectangle, _, _) = MatUtilities.Find(Mat, GetAnchor(screenName, anchorName));

            Log(mat => CvInvoke.Rectangle(mat, rectangle, new MCvScalar(0)), "Find");

            return (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        protected bool IsExists(string screenName, string anchorName)
        {
            var (rectangle, threshold, isExists) = MatUtilities.Find(Mat, GetAnchor(screenName, anchorName));

            Log(mat => CvInvoke.Rectangle(mat, rectangle, new MCvScalar(0)), $"IsExists_{threshold}");

            return isExists;
        }

        protected static void Sleep(int millisecondsTimeout) => Thread.Sleep(millisecondsTimeout);
        protected static void MoveAndClick(int x, int y, int dx = 0, int dy = 0) => MouseUtilities.MoveAndClick(x + dx, y + dy);

        protected static void Paste(string text, int sleep)
        {
            ClipboardUtilities.Paste(text);
            Sleep(sleep);
        }

        public bool OnAction(Mat mat)
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

        protected abstract bool OnActionInternal(Mat mat);
        public abstract string CurrentActionName { get; }
    }
}
