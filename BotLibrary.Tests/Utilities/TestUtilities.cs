using System.IO;
using NUnit.Framework;

namespace BotLibrary.Tests.Utilities
{
    public static class TestUtilities
    {
        public static string OutputDirectory => TestContext.CurrentContext.TestDirectory;
        public static string ProjectDirectory => Path.GetDirectoryName(Path.GetDirectoryName(OutputDirectory));
        public static string SolutionDirectory => Path.GetDirectoryName(ProjectDirectory);
        public static string TestDataDirectory => Path.Combine(ProjectDirectory, "TestData");
        public static string ScreenshotsDirectory => Path.Combine(TestDataDirectory, "screenshots");
        public static string AnchorsDirectory => Path.Combine(SolutionDirectory, "anchors");
    }
}
