using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace KakaoTalkBot.Utilities
{
    public static class Logger
    {
        public static ILog Log { get; } = InitLogger();

        public static ILog InitLogger()
        {
            XmlConfigurator.Configure();

            return LogManager.GetLogger("LOGGER");
        }

        public static string FilePath
        {
            get
            {
                var rootAppender = Log
                    .Logger
                    .Repository
                    .GetAppenders().OfType<FileAppender>()
                    .FirstOrDefault();

                return rootAppender != null ? rootAppender.File : string.Empty;
            }
        }
    }
}
