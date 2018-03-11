using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KakaoTalkBotLibrary.Utilities
{
    public class Anchors : Dictionary<string, Anchor>
    {
        #region Properties

        private static string MetadataFileName { get; } = "anchors.json";
        private static string GetMetadataPath(string folder) => Path.Combine(folder, MetadataFileName);

        #endregion

        #region Public methods

        public void Load(string folder)
        {
            var images = MatUtilities.LoadImages(folder, "*.bmp");

            var path = GetMetadataPath(folder);
            var anchors = File.Exists(path)
                ? JsonConvert.DeserializeObject<Dictionary<string, Anchor>>(File.ReadAllText(path))
                : new Dictionary<string, Anchor>();

            AddValues(images
                .Select(pair => anchors.TryGetValue(pair.Key, out var result) ? result.WithMat(pair.Value) : new Anchor
                {
                    Name = pair.Key,
                    Mat = pair.Value
                }));
        }

        private void AddValue(Anchor anchor)
        {
            this[anchor.Name] = anchor;
        }

        private void AddValues(IEnumerable<Anchor> anchors)
        {
            foreach (var anchor in anchors)
            {
                AddValue(anchor);
            }
        }

        public void Save(string folder)
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            var path = GetMetadataPath(folder);

            File.WriteAllText(path, text);
        }

        public Anchor GetAnchor(string name) => TryGetValue(name, out var result) ? result : null;

        #endregion

    }
}
