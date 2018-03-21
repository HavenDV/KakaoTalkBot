using System;
using System.Drawing;
using BotLibrary.Utilities;

namespace BotLibrary.Extensions
{
    public static class RectExtensions
    {
        private static int ToInt(double value) => (int)Math.Round(value);

        public static Rectangle TransformForSize(this Rectangle rectangle, Size size, Size baseSize, Offsets offsets)
        {
            var (fx, fy) = size.ApplyOffsets(offsets).GetTransformKoefs(baseSize.ApplyOffsets(offsets));

            var rect = rectangle.ApplyOffsets(offsets);

            var x1 = ToInt(rect.X * fx);
            var y1 = ToInt(rect.Y * fy);
            var x2 = ToInt((rect.X + rectangle.Width) * fx);
            var y2 = ToInt((rect.Y + rectangle.Height) * fy);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1).ReturnOffsets(offsets);
        }

        public static Rectangle ApplyOffsets(this Rectangle rectangle, Offsets offsets)
        {
            return new Rectangle(rectangle.X - offsets.Left, rectangle.Y - offsets.Top, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ReturnOffsets(this Rectangle rectangle, Offsets offsets)
        {
            return new Rectangle(rectangle.X + offsets.Left, rectangle.Y + offsets.Top, rectangle.Width, rectangle.Height);
        }
    }
}
