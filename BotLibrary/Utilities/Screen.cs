using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BotLibrary.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Newtonsoft.Json;

namespace BotLibrary.Utilities
{
    public class Screen : IDisposable
    {
        public string Name { get; set; }

        [JsonIgnore]
        public Mat Mat { get; set; }

        public Screen WithMat(Mat mat)
        {
            Mat = mat;

            return this;
        }

        public List<Anchor> Anchors { get; set; } = new List<Anchor>();

        public (double fx, double fy) GetAnchorKoefs(Anchor anchor, Size size = default(Size))
        {
            if (size == Size.Empty)
            {
                size = Mat.Size;
            }

            var fx = 1.0 * size.Width / Mat.Width;
            var fy = 1.0 * size.Height / Mat.Height;

            return (fx, fy);
        }

        private int ToInt(double value) => (int)Math.Round(value);

        public Rectangle GetAnchorRectangle(Anchor anchor, Size size = default(Size))
        {
            var (fx, fy) = GetAnchorKoefs(anchor, size);
            var rect = anchor.Rectangle;

            const int titleHeight = 30;
            var y = rect.Y - titleHeight;

            var x1 = ToInt(rect.X * fx);
            var y1 = ToInt(y * fy) + titleHeight;
            var x2 = ToInt((rect.X + rect.Width) * fx);
            var y2 = ToInt((y + rect.Height) * fy) + titleHeight;

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public Rectangle GetAnchorRectangle(string name, Size size = default(Size)) =>
            GetAnchorRectangle(GetAnchorByName(name), size);

        public Mat GetAnchorMat(Anchor anchor, Size size = default(Size))
        {
            var anchorMat = new Mat(Mat, anchor.Rectangle);
            var (fx, fy) = GetAnchorKoefs(anchor, size);
            var resizedAnchorMat = anchorMat.Resize(Size.Empty, fx, fy, Inter.Area);

            return resizedAnchorMat;
        }

        public Anchor GetAnchorByName(string name) => Anchors
            ?.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"Anchor is not found: {name}");

        public Mat GetAnchorMat(string name, Size size = default(Size)) => 
            GetAnchorMat(GetAnchorByName(name), size);

        public (string name, Mat mat)[] GetAnchors(Size size = default(Size))
        {
            return Anchors.Select(i => (i.Name, GetAnchorMat(i, size))).ToArray();
        }

        #region IDisposable

        public void Dispose()
        {
            Mat?.Dispose();
            Mat = null;
        }

        #endregion
    }
}
