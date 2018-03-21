using System.Drawing;
using BotLibrary.Utilities;

namespace BotLibrary.Extensions
{
    public static class SizeExtensions
    {
        public static (double fx, double fy) GetTransformKoefs(this Size size, Size baseSize)
        {
            var fx = 1.0 * size.Width / baseSize.Width;
            var fy = 1.0 * size.Height / baseSize.Height;

            return (fx, fy);
        }

        public static Size ApplyOffsets(this Size size, Offsets offsets)
        {
            return new Size(size.Width - offsets.Left - offsets.Right, size.Height - offsets.Top - offsets.Bottom);
        }
    }
}
