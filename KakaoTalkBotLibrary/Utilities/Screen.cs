using System.Collections.Generic;
using Emgu.CV;
using Newtonsoft.Json;

namespace KakaoTalkBotLibrary.Utilities
{
    public class Screen
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
    }
}
