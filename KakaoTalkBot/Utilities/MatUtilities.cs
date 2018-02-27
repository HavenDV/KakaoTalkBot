using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace KakaoTalkBot.Utilities
{
    public static class MatUtilities
    {
        public static Dictionary<string, Mat> LoadAnchors(string folder, string extension = "*.*")
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                return new Dictionary<string, Mat>();
            }

            return Directory
                .EnumerateFiles(folder, extension)
                .ToDictionary(Path.GetFileName, entry => new Mat(entry));
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
    }
}
