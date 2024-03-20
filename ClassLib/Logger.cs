using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Diagnostics;

namespace ClassLib
{
    public class Logger
    {
        public readonly static string logPath;

        private readonly static ILogger<Logger> logger;

        static Logger()
        {
            logPath = HelpingMethods.LogPath + $"{Path.DirectorySeparatorChar}Log-{DateTime.Now:dd.MM.yy}.txt";
            if (!File.Exists(logPath))
            {
                var stream = File.Create(logPath);
                stream.Close();
            }

            using (StreamWriter logFileWriter = new StreamWriter(logPath, append: true))
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });

                logger = loggerFactory.CreateLogger<Logger>();
            }
        }

        public static void WriteLog(string name, string condition)
        {
            string log = $"{name} {condition} at {DateTime.Now}";
            logger.LogInformation(log);
            using (var logWriter = new StreamWriter(logPath, true))
            {
                logWriter.WriteLine(log);
            }
        }
        public static void WriteExceptionLog(string name, Exception exception)
        {
            string log = $"An exception has been thrown in {name} at {DateTime.Now}";
            logger.LogError(log);
            using (var logWriter = new StreamWriter(logPath, true))
            {
                logWriter.WriteLine(log);
            }
        }
    }
}

