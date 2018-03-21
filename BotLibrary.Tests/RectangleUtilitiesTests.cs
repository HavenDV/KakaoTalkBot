using System.Drawing;
using BotLibrary.Extensions;
using BotLibrary.Utilities;
using NUnit.Framework;

namespace BotLibrary.Tests
{
    [TestFixture]
    public class RectangleUtilitiesTests
    {
        #region Tests

        [Test]
        public void IsExistsTest()
        {
            var size = new Size(116, 220);
            var baseSize = new Size(655, 900);
            var offsets = new Offsets(2, 30, 2, 2);

            BaseTest(new Rectangle(222, 60, 365, 49), size, baseSize, offsets, new Rectangle(40, 36, 63, 11));
            BaseTest(new Rectangle(609, 848, 39, 44), size, baseSize, offsets, new Rectangle(106, 207, 7, 10));
        }

        #endregion

        #region Base Tests

        private static void BaseTest(Rectangle rectangle, Size size, Size baseSize, Offsets offsets, Rectangle expected)
        {
            var actual = rectangle.TransformForSize(size, baseSize, offsets);

            Assert.AreEqual(expected, actual);
        }

        #endregion

    }
}
