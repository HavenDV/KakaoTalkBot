using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace KakaoTalkBotLibrary.Extensions
{
    public static class MatExtensions
    {
        public static Mat ToGray(this Mat mat) =>
            mat.Modify(dst => CvInvoke.CvtColor(mat, dst, ColorConversion.Bgra2Gray));

        public static Mat Resize(this Mat mat, Size size) => 
            mat.Modify(dst => CvInvoke.Resize(mat, dst, size));

        public static Mat Modify(this Mat mat, Action<Mat> action)
        {
            var dstMat = new Mat();
            action?.Invoke(dstMat);

            mat.Dispose();

            return dstMat;
        }
    }
}
