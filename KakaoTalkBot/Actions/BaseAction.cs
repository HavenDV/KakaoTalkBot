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
        public DateTime DateTime { get; } = DateTime.Now;

        public int PasteTimeout { get; set; } = 50;
        public int FieldClickTimeout { get; set; } = 50;

        public delegate void TextDelegate(string text);
        public event TextDelegate NewLog;
        protected void Log(string text) => NewLog?.Invoke(text);

        public ApplicationInfo Info { get; set; }
        protected Mat GetAnchor(string screenName, string anchorName) =>
            Info.GetAnchor(screenName, anchorName, Mat.Size) ??
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
                var span = DateTime.Now.Subtract(DateTime);
                mat.Save($"D:/logs/log_{prefix}_{new Random().Next()}_{span.TotalMilliseconds:F0}.png");
            }
        }

        protected (int x, int y, int w, int h) Find(string screenName, string anchorName)
        {
            var (rectangle, _, _) = MatUtilities.Find(Mat, GetAnchor(screenName, anchorName));

            Log(mat => CvInvoke.Rectangle(mat, rectangle, new MCvScalar(0)), $"Find_{screenName}_{anchorName}");

            return (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        protected bool IsExists(string screenName, string anchorName, out (int x, int y, int w, int h) tuple)
        {
            var rectangle1 = Info.GetAnchorRectangle(screenName, anchorName, Mat.Size);
            tuple = (rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height);
            Log(mat => CvInvoke.Rectangle(mat, rectangle1, new MCvScalar(0)), $"IsExists_{screenName}_{anchorName}");
            return true;

            var (rectangle, threshold, isExists) = MatUtilities.Find(Mat, GetAnchor(screenName, anchorName));

            Log(mat => CvInvoke.Rectangle(mat, rectangle, new MCvScalar(0)), $"IsExists_{screenName}_{anchorName}_{threshold}");

            tuple = (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            return isExists;
        }

        protected static void Sleep(int millisecondsTimeout) => Thread.Sleep(millisecondsTimeout);
        protected static void MoveAndClick(int x, int y, int dx = 0, int dy = 0) => MouseUtilities.MoveAndClick(x + dx, y + dy);

        protected static void MoveAndClick((int x, int y, int w, int h) tuple, double fx, double fy) =>
            MoveAndClick(tuple.x, tuple.y, (int)Math.Round(fx * tuple.w), (int)Math.Round(fy * tuple.h));

        protected void Paste(string text)
        {
            ClipboardUtilities.Paste(text);
            Sleep(PasteTimeout);
        }

        public bool OnAction(Mat mat)
        {
            Mat = mat;

            return OnActionInternal();
        }

        protected abstract bool OnActionInternal();
        public abstract string CurrentActionName { get; }
    }
}
