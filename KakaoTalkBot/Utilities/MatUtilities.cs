using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using KakaoTalkBot.Extensions;
using Pranas;

namespace KakaoTalkBot.Utilities
{
    public static class MatUtilities
    {
        public static Dictionary<string, Mat> LoadAnchors(string folder, string extension = "*.*")
        {
            if (String.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                return new Dictionary<string, Mat>();
            }

            return Directory
                .EnumerateFiles(folder, extension)
                .ToDictionary(Path.GetFileName, entry => new Mat(entry).ToGray());
        }

        public static (int, int, int, int) Find(IInputArray mat, Mat obj)
        {
            using (var resultMat = new Mat())
            {
                CvInvoke.MatchTemplate(mat, obj, resultMat, TemplateMatchingType.Sqdiff);
                resultMat.MinMax(out _, out _, out var points, out _);
                var x = points[0].X;
                var y = points[0].Y;

                return (x, y, obj.Width, obj.Height);
            }
        }

        public static bool IsExists(IInputArray mat, Mat obj)
        {
            using (var resultMat = new Mat())
            {
                CvInvoke.MatchTemplate(mat, obj, resultMat, TemplateMatchingType.Sqdiff);
                resultMat.MinMax(out var values, out _, out var _, out _);
                var value = values[0];

                return value / resultMat.Rows / resultMat.Cols < 3.0;
            }
        }

        public static Mat GetScreenshot() => ToMat(ScreenshotCapture.TakeScreenshot(true));

        private static Mat ToMat(Image image)
        {
            using (image)
            {
                return ToMat(new Bitmap(image));
            }
        }

        private static Mat ToMat(Bitmap bitmap)
        {
            using (bitmap)
            using (var mat = bitmap.ToMat())
            {
                return mat.ToGray();
            }
        }

        public static (Win32.Rect, Mat)[] GetScreenshotOfProcess(string name) =>
            WindowsUtilities.GetScreenshotOfProcess(name).Select(i => (i.Item1, ToMat(i.Item2))).ToArray();
    }
}
