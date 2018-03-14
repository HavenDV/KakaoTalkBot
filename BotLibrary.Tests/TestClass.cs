using System;
using System.Collections.Generic;
using System.Linq;
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
            IsExistsBaseTest("add_by_contacts_max.bmp", "add_by_contacts");
            IsExistsBaseTest("find_max.bmp", "find");
        }

        #endregion

        #region Base Tests

        private static void IsExistsBaseTest(string screenName, string prefix)
        {
            foreach (var pair in GetImagesWithPrefix(Screenshots, prefix))
            {
                foreach (var (anchorName, anchor) in Screens.GetAnchors(screenName, pair.Value.Size))
                {
                    var (threshold, isExists) = MatUtilities.IsExistsEx(pair.Value, anchor);
                    if (!isExists)
                    {
                        Assert.Warn($"!MatUtilities.IsExists: Anchor: {anchorName}. Image: {pair.Key}. Threshold: {threshold}. Edge threshold: {MatUtilities.IsExistsThreshold}");
                    }
                }
            }
        }

        #endregion

        #region Utilities

        private static Dictionary<string, Mat> GetImagesWithPrefix(Dictionary<string, Mat> dictionary, string prefix) =>
            dictionary
                .Where(pair => pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        #endregion

    }
}
