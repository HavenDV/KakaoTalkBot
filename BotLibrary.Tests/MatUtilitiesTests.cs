using System;
using System.Collections.Generic;
using System.Linq;
using BotLibrary.Tests.Utilities;
using BotLibrary.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using NUnit.Framework;

namespace BotLibrary.Tests
{
    [TestFixture]
    public class MatUtilitiesTests
    {
        #region Properties

        private static ApplicationInfo Info { get; } = new ApplicationInfo(TestUtilities.AnchorsDirectory);
        private static Dictionary<string, Mat> Screenshots { get; } = MatUtilities.LoadImages(TestUtilities.ScreenshotsDirectory);

        #endregion

        #region Tests

        [Test]
        public void IsExistsTest()
        {
            IsExistsBaseTest("add_by_contacts.bmp", "add_by_contacts");
            IsExistsBaseTest("find_subtab.bmp", "find_subtab");
            IsExistsBaseTest("select_phone_code_normal.bmp", "select_phone_code_normal");
            IsExistsBaseTest("select_phone_code_afterpaste.bmp", "select_phone_code_afterpaste");
        }

        #endregion

        #region Base Tests

        private static void IsExistsBaseTest(string screenName, string prefix)
        {
            foreach (var pair in GetImagesWithPrefix(Screenshots, prefix))
            {
                foreach (var (anchorName, anchor) in Info.GetAnchors(screenName, pair.Value.Size))
                {
                    var (rectangle, threshold, isExists) = MatUtilities.Find(pair.Value, anchor);
                    if (!isExists)
                    {
                        Assert.Warn($"!MatUtilities.IsExists: Anchor: {anchorName}. Image: {pair.Key}. Threshold: {threshold}. Edge threshold: {MatUtilities.IsExistsThreshold}");

                        using (var mat = pair.Value.Clone())
                        {
                            CvInvoke.Rectangle(mat, rectangle, new MCvScalar(0));
                            CvInvoke.Imshow("fail", mat);
                            CvInvoke.WaitKey();
                        }
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
