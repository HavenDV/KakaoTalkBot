using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BotLibrary.Extensions;
using BotLibrary.Tests.Utilities;
using BotLibrary.Utilities;
using Emgu.CV;
using NUnit.Framework;

namespace BotLibrary.Tests
{
    [TestFixture]
    public class TestClass
    {
        #region Properties

        private static Screens Screens { get; } = new Screens(TestUtilities.AnchorsDirectory);
        private static Dictionary<string, Mat> Screenshots { get; } = MatUtilities.LoadImages(TestUtilities.ScreenshotsDirectory);

        #endregion

        #region Tests

        [Test]
        public void TestMethod()
        {
            IsExistsBaseTest("add_by_contacts_max.bmp", "Back", "add_by_contacts");
        }

        #endregion

        #region Base Tests

        private static void IsExistsBaseTest(string screenName, string anchorName, string prefix)
        {
            var (screen, anchor) = GetAnchor(anchorName, screenName, Screens);
            foreach (var pair in GetImagesWithPrefix(prefix, Screenshots))
            {
                var anchorMat = new Mat(screen.Mat, anchor.Rectangle);

                var fx = 1.0 * pair.Value.Width / screen.Mat.Width;
                var fy = 1.0 * pair.Value.Height / screen.Mat.Height;
                var resizedAnchorMat = anchorMat.Resize(Size.Empty, fx, fy);

                var result = MatUtilities.IsExistsEx(pair.Value, resizedAnchorMat);
                if (!result.isExists)
                {
                    Assert.Warn($"!MatUtilities.IsExists: Anchor: {anchorName}. Image: {pair.Key}. Threshold: {result.threshold}. Edge threshold: {MatUtilities.IsExistsThreshold}");
                }
            }
        }

        #endregion

        #region Utilities

        private static (Screen screen, Anchor anchor) GetAnchor(string name, string screenName, Screens screens)
        {
            var screen = screens.FirstOrDefault(i => string.Equals(i.Name, screenName, StringComparison.OrdinalIgnoreCase));
            var anchor = screen
                             ?.Anchors
                             ?.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase)) ??
                             throw new FileNotFoundException($"Anchor is not found: {name}");


            return (screen, anchor);
        }

        private static Dictionary<string, Mat> GetImagesWithPrefix(string prefix, Dictionary<string, Mat> dictionary) =>
            dictionary
                .Where(pair => pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        #endregion

    }
}
