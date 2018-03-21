using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using BotLibrary.Extensions;
using Emgu.CV;
using Newtonsoft.Json;

namespace BotLibrary.Utilities
{
    public class ApplicationInfo : IDisposable, INotifyPropertyChanged
    {
        #region Properties

        private static string MetadataFileName { get; } = "screens.json";

        private static string GetMetadataPath(string folder) => Path.Combine(folder, MetadataFileName);

        private Offsets _offsets = new Offsets();
        public Offsets Offsets
        {
            get => _offsets;
            set
            {
                _offsets = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offsets)));
            }
        }

        public List<Screen> Screens { get; } = new List<Screen>();

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ApplicationInfo()
        {
        }

        public ApplicationInfo(string folder)
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
            var info = File.Exists(path)
                ? JsonConvert.DeserializeObject<ApplicationInfo>(File.ReadAllText(path))
                : new ApplicationInfo();
            
            Screens.Dispose();
            Screens.Clear();
            AddValues(images
                .Select(pair => info.GetScreenOrDefault(pair.Key)?.WithMat(pair.Value) ?? new Screen
                {
                    Name = pair.Key,
                    Mat = pair.Value
                }));

            Offsets = info.Offsets;
        }

        private void AddValues(IEnumerable<Screen> screens)
        {
            foreach (var screen in screens)
            {
                Screens.Add(screen);
            }
        }

        public void Save(string folder)
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            var path = GetMetadataPath(folder);

            File.WriteAllText(path, text);
        }

        public Screen GetScreenOrDefault(string name) => Screens
            .FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));

        public Screen GetScreen(string name) => GetScreenOrDefault(name) ?? throw new Exception($"Screen is not found: {name}");

        public Mat GetAnchor(string screenName, string anchorName, Size size) => GetScreen(screenName)
            ?.GetAnchorMat(anchorName, size);

        public Rectangle GetAnchorRectangle(string screenName, string anchorName, Size size) => GetScreen(screenName)
            ?.GetAnchorRectangle(anchorName, Offsets, size) ?? Rectangle.Empty;

        public (string name, Mat mat)[] GetAnchors(string screenName, Size size) => GetScreen(screenName)
            ?.GetAnchors(size);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Screens.Dispose();
        }

        #endregion
    }
}
