using System.Drawing;
using Emgu.CV;
using Newtonsoft.Json;

namespace KakaoTalkBotLibrary.Utilities
{
    public class Anchor
    {
        public string Name { get; set; }

        [JsonIgnore]
        public Mat Mat { get; set; }

        public Size SourceResolution { get; set; }

        public Anchor WithMat(Mat mat)
        {
            Mat = mat;

            return this;
        }
    }
}
