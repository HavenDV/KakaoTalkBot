using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BotLibrary.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace BotLibrary.Utilities
{
    public static class MatUtilities
    {
        public static double IsExistsThreshold { get; } = 3.0;

        public static Dictionary<string, Mat> LoadImages(string folder, string extension = "*.*")
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
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

        public static (double threshold, bool isExists) IsExistsEx(IInputArray mat, Mat obj)
        {
            using (var resultMat = new Mat())
            {
                CvInvoke.MatchTemplate(mat, obj, resultMat, TemplateMatchingType.Sqdiff);
                resultMat.MinMax(out var values, out _, out var _, out _);
                var value = values[0];

                var threshold = value / resultMat.Rows / resultMat.Cols;
                return (threshold, threshold < IsExistsThreshold);
            }
        }

        public static (double threshold, bool isExists) IsExistsMultuEx(Mat mat, Mat obj)
        {
            var results = new List<(double, bool)>();
            for (var i = 1; i < mat.Width; i+= 100)
            {
                //for (var j = 1; j < mat.Height; j++)
                {
                    var resizedObj = obj.Resize(new Size(i, obj.Height));

                    //results.Add(IsExistsEx(mat, resizedObj));
                }
            }

            return results.OrderBy(i => i.Item1).FirstOrDefault();
        }

        public static bool IsExists(IInputArray mat, Mat obj) => IsExistsEx(mat, obj).Item2;

        public static Mat ToMat(Image image)
        {
            using (image)
            {
                return ToMat(new Bitmap(image));
            }
        }

        public static Mat ToMat(Bitmap bitmap)
        {
            using (bitmap)
            using (var mat = bitmap.ToMat())
            {
                return mat.ToGray();
            }
        }
    }
}
