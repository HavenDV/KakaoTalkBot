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
        public static double IsExistsThreshold { get; } = 1.0;

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

        public static (Rectangle rectangle, double threshold, bool isExists) Find(IInputArray mat, Mat obj)
        {
            using (var resultMat = new Mat())
            {
                CvInvoke.MatchTemplate(mat, obj, resultMat, TemplateMatchingType.SqdiffNormed);
                resultMat.MinMax(out var values, out _, out var points, out _);
                var x = points[0].X;
                var y = points[0].Y;
                var value = values[0];
                var threshold = value;
                var rectangle = new Rectangle(x, y, obj.Width, obj.Height);

                return (rectangle, threshold, threshold < IsExistsThreshold);
            }
        }

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
