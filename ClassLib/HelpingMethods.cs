using Smth;
using System;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ClassLib
{
    public partial class HelpingMethods
    {
        internal static List<AeroexpressTable> currentAeroexpressTable;

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

        internal static void GetPathForFile()
        {
            Process process = Process.GetCurrentProcess();
            CSVProcessing.dataPath = process.MainModule.FileName;
            CSVProcessing.dataPath = Path.GetDirectoryName(CSVProcessing.dataPath);
            CSVProcessing.dataPath = Path.GetDirectoryName(CSVProcessing.dataPath);
            CSVProcessing.dataPath = Path.GetDirectoryName(CSVProcessing.dataPath);
            CSVProcessing.dataPath = Path.GetDirectoryName(CSVProcessing.dataPath);
            CSVProcessing.dataPath += "\\Data";
            if (!Directory.Exists(CSVProcessing.dataPath))
            {
                Directory.CreateDirectory(CSVProcessing.dataPath);
            }
        }

        internal static async Task DownloadData(Update update)
        {
            GetPathForFile();
            string fileName = update.Message.Document.FileName;

            if (fileName.EndsWith(".csv"))
            {
                BotUpdates.lastCsvDownload = await CSVProcessing.DownloadFile(BotUpdates.botClient, update);
                var temporaryTable = CSVProcessing.Read(BotUpdates.lastCsvDownload);
                BotUpdates.lastCsvUpload = CSVProcessing.Write();
            }
            else if (fileName.EndsWith(".json"))
            {
                //BotUpdates.lastJsonDownload = await
                //var temporaryTable = 
                //BotUpdates.lastJsonUpload = await
            }
            else
            {
                await BotUpdates.botClient.SendTextMessageAsync(update.Message.Chat.Id, "Некорректное расширение файла");
            }
        }
    }
}

