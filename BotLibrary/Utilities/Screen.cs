using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        public Rectangle GetAnchorRectangle(Anchor anchor, Offsets offsets, Size size = default(Size))
        {
            offsets = offsets ?? new Offsets();

            var result = anchor.Rectangle.TransformForSize(size, Mat.Size, offsets);
            //File.WriteAllLines("D:/test2.txt", File.ReadAllLines("D:/test2.txt").Concat(new List<string>
            //{
            //    $"Rectangle: {anchor.Rectangle}, Size: {size}, BaseSize: {Mat.Size}, Offsets: {offsets}, Result: {result}"
            //}));

            return result;
        }

        public Rectangle GetAnchorRectangle(string name, Offsets offsets, Size size = default(Size)) =>
            GetAnchorRectangle(GetAnchorByName(name), offsets, size);

        public Mat GetAnchorMat(Anchor anchor, Size size = default(Size))
        {
            if (size == Size.Empty)
            {
                size = Mat.Size;
            }

            var anchorMat = new Mat(Mat, anchor.Rectangle);
            var (fx, fy) = size.GetTransformKoefs(Mat.Size);
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
