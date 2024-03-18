using Smth;
using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ClassLib
{
    public partial class HelpingMethods
    {
        internal static List<AeroexpressTable> currentAeroexpressTableCsv;

        internal static List<AeroexpressTable> currentAeroexpressTableJson;

        internal static string filePath;


        /// <summary>
        /// Check for correctness and return user's input.
        /// </summary>
        /// <returns>So useful ConsoleKey from user's input.</returns>
        internal static ConsoleKey ItemForSorting()
        {
            var key = Console.ReadKey().Key;
            while (key != ConsoleKey.D1 && key != ConsoleKey.D2 && key != ConsoleKey.D3 && key != ConsoleKey.D4
                && key != ConsoleKey.D5 && key != ConsoleKey.D6)
            {
                Console.WriteLine();
                PrintWithColor("Incorrect input, try again: ", ConsoleColor.Red);
                key = Console.ReadKey().Key;
            }
            return key;
        }

        internal static string GetPathForFile()
        {
            Process process = Process.GetCurrentProcess();
            string path = process.MainModule.FileName;
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path = Path.GetDirectoryName(path);
            path += "\\Data";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            filePath = path;
            return path;
        }

        internal static async Task DownloadData(Update update)
        {
            GetPathForFile();
            string fileName = update.Message.Document.FileName;

            if (fileName.EndsWith(".csv"))
            {
                var path = await CSVProcessing.DownloadFile(BotUpdates.botClient, update, filePath);
                filePath = path;
                var temporaryTable = CSVProcessing.Read().ToList();
                await BotUpdates.botClient.SendTextMessageAsync(update.Message.Chat.Id, "Данные успешно загружены!");
            }
            else if (fileName.EndsWith(".json"))
            {

            }
            else
            {
                await BotUpdates.botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректное расширение файла");
            }
        }
    }
}

