using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emgu.CV;
using KakaoTalkBotLibrary.Tests.Utilities;
using KakaoTalkBotLibrary.Utilities;
using NUnit.Framework;

namespace KakaoTalkBotLibrary.Tests
{
    [TestFixture]
    public class TestClass
    {
        #region Properties

        private static Dictionary<string, Mat> Anchors { get; } = MatUtilities.LoadImages(TestUtilities.AnchorsDirectory);
        private static Dictionary<string, Mat> Screenshots { get; } = MatUtilities.LoadImages(TestUtilities.ScreenshotsDirectory);

        #endregion

        #region Tests

        [Test]
        public void TestMethod()
        {
            IsExistsBaseTest("find_menu.bmp", "find");
        }

        #endregion

        #region Base Tests

        private static void IsExistsBaseTest(string anchorName, string prefix)
        {
            var anchor = GetAnchor(anchorName, Anchors);
            foreach (var pair in GetImagesWithPrefix(prefix, Screenshots))
            {
                if (!MatUtilities.IsExists(pair.Value, anchor))
                {
                    Assert.Warn($"!MatUtilities.IsExists: Anchor: {anchorName}. Image: {pair.Key}. Threshold: {MatUtilities.IsExistsThreshold}");
                }
            }
        }

        #endregion

        #region Utilities

        private static Mat GetAnchor(string name, IReadOnlyDictionary<string, Mat> dictionary) =>
            dictionary.TryGetValue(name, out var result)
                ? result
                : throw new FileNotFoundException($"Anchor is not found: {name}");

        private static Dictionary<string, Mat> GetImagesWithPrefix(string prefix, Dictionary<string, Mat> dictionary) =>
            dictionary
                .Where(pair => pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        #endregion

    }
}
