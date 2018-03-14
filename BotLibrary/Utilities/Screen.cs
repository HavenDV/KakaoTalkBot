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

        public Mat GetAnchorMat(Anchor anchor, Size size = default(Size))
        {
            if (size == Size.Empty)
            {
                size = Mat.Size;
            }

            var anchorMat = new Mat(Mat, anchor.Rectangle);
            var fx = 1.0 * size.Width / Mat.Width;
            var fy = 1.0 * size.Height / Mat.Height;
            var resizedAnchorMat = anchorMat.Resize(Size.Empty, fx, fy, Inter.Area);

            return resizedAnchorMat;
        }

        public Mat GetAnchor(string name, Size size = default(Size))
        {
            var anchor = Anchors
                         ?.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase)) ??
                         throw new Exception($"Anchor is not found: {name}");

            return GetAnchorMat(anchor, size);
        }

        public (string name, Mat mat)[] GetAnchors(Size size = default(Size))
        {
            return Anchors.Select(i => (i.Name, GetAnchor(i.Name, size))).ToArray();
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
