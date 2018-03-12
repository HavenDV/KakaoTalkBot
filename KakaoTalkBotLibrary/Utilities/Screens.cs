using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KakaoTalkBotLibrary.Utilities
{
    public class Screens : List<Screen>
    {
        #region Properties

        private static string MetadataFileName { get; } = "screens.json";
        private static string GetMetadataPath(string folder) => Path.Combine(folder, MetadataFileName);

        #endregion

        #region Constructors

        public Screens()
        {
        }

        public Screens(string folder)
        {
            Load(folder);
        }

        #endregion

        #region Public methods

        public void Load(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException();
            }

            var images = MatUtilities.LoadImages(folder, "*.bmp");

            var path = GetMetadataPath(folder);
            var screens = File.Exists(path)
                ? JsonConvert.DeserializeObject<List<Screen>>(File.ReadAllText(path))
                : new List<Screen>();

            AddValues(images
                .Select(pair => screens.FirstOrDefault(i => string.Equals(pair.Key, i.Name, StringComparison.OrdinalIgnoreCase))
                    ?.WithMat(pair.Value) ?? new Screen
                {
                    Name = pair.Key,
                    Mat = pair.Value
                }));
        }

        private void AddValues(IEnumerable<Screen> screens)
        {
            foreach (var screen in screens)
            {
                Add(screen);
            }
        }

        public void Save(string folder)
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            var path = GetMetadataPath(folder);

            File.WriteAllText(path, text);
        }

        //public Anchor GetAnchor(string name) => TryGetValue(name, out var result) ? result : null;

        #endregion

    }
}
