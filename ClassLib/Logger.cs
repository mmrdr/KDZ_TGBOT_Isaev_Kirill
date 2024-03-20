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
    /// <summary>
    /// Класс, реализующий логгирование.
    /// </summary>
    public class Logger
    {
        public readonly static string logPath;

        private readonly static ILogger<Logger> logger;
        /// <summary>
        /// Статический конструктор, чтобы единожды создать логгер, для его дальнейшей работы.
        /// </summary>
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
        /// <summary>
        /// Записывает все логи в файл, располагающийся в logPath.
        /// </summary>
        /// <param name="name">Имя метода.</param>
        /// <param name="condition">Состояние метода.</param>
        public static void WriteLog(string name, string condition)
        {
            string log = $"{name} {condition} в {DateTime.Now}";
            logger.LogInformation(log);
            using (var logWriter = new StreamWriter(logPath, true))
            {
                logWriter.WriteLine(log);
            }
        }
        /// <summary>
        /// Записывает все логи всех исключений в файл, располагающийся в logPath.
        /// </summary>
        /// <param name="name">Имя метода.</param>
        /// <param name="exception">Состояние метода.</param>
        public static void WriteExceptionLog(string name, Exception exception)
        {
            string log = $"Исключение было вызвано в {name} в {DateTime.Now}";
            logger.LogError(log);
            using (var logWriter = new StreamWriter(logPath, true))
            {
                logWriter.WriteLine(log);
            }
        }
    }
}

